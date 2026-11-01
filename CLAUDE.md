# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## 프로젝트 개요

Cookie Zephyros의 일본어 원본 `gvtrademap_cs`(대항해시대Online 교역MAP C#)를 한국어판 게임에 맞게 포크·현지화한 저장소다. Windows Forms 셸 위에서 Managed DirectX 1.1로 세계지도를 그리는 오버레이 도구로, 게임 클라이언트 창을 화면 캡처해 자기 배의 위치·방향을 읽고, 채팅 로그를 파싱해 해역 변동 등을 추적한다.

## 빌드

- 솔루션: `gvtrademap_cs/gvtrademap_cs.sln` (앱 1개 + `library_cs` 하위 6개 프로젝트)
- 타깃: .NET Framework **v2.0**, PlatformTarget **x86**, WinExe. 솔루션 포맷은 VS14(2015) 기준이며 원본 README는 VS2008 기준이라고 적혀 있다.
- 필수 의존: **Managed DirectX 1.1**(`C:\Windows\Microsoft.NET\DirectX for Managed Code\`). 없으면 실행 시 `assembly_load_error_form`이 뜬다.

```powershell
# Visual Studio가 있을 때
msbuild gvtrademap_cs\gvtrademap_cs.sln /p:Configuration=Debug /p:Platform="Any CPU"
```

현재 개발 머신 상태(확인함): Visual Studio·Windows SDK가 설치돼 있지 않아 `C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe`로 빌드하면 `utility.csproj`에서 **MSB3091 (resgen.exe 없음)**으로 실패한다. 빌드하려면 VS 또는 Windows SDK 7.0A(NetFx35 Tools)가 필요하다. 빌드 불가 상태에서 "빌드해서 확인했다"고 보고하지 말 것.

대신 **컴파일 검사**는 가능하다: dotnet SDK 의 Roslyn(`C:\Program Files\dotnet\sdk\<ver>\Roslyn\bincore\csc.dll`)으로 각 csproj 의 `Compile` 목록을 의존 순서(utility → PanelManager → net → gvo_base → directx → gvo_net_base → 앱)대로 `-nostdlib+ -langversion:6 -platform:x86 -define:DEBUG;TRACE -unsafe+`, 참조는 `Framework\v2.0.50727\*.dll` 과 MDX 1.1 DLL(Direct3DX 만 1.0.2911.0, 나머지 1.0.2902.0)로 넘기면 된다. resx 는 처리되지 않으므로 "컴파일 확인"이지 "빌드 확인"이 아니다. 만든 `gvo_base.dll` 등은 CLR 2.0(`Framework\v2.0.50727`)이 있어 작은 콘솔 프로그램으로 DB 로딩·매칭을 실제 실행해 볼 수 있다. 소스의 C# 버전은 6(`using static` 등 사용)이다.

그 밖의 빌드 관련 사실:
- 포스트빌드에서 ILMerge(`c:\Program Files (x86)\Microsoft\ILMerge\ILMerge.exe`)로 `gvtrademap_cs_.exe` + 각 DLL을 `gvtrademap_cs.exe` 하나로 합친다. ILMerge가 없으면 이 단계만 실패하고 산출물은 `gvtrademap_cs_.exe` + DLL들로 남는다.
- 셰이더는 자동 빌드되지 않는다. `library_cs/directx/fx/sprite.fx`를 고치면 같은 폴더의 `fxc_cnv.bat`(fxc `/Tfx_2_0`)를 직접 돌려 `sprite.fxo`를 갱신해야 한다. `.fxo`는 directx 어셈블리에 `directx.fx.sprite.fxo`로 임베드된다.
- Release로 빌드할 때는 `bin\Debug` 안의 데이터 폴더들을 `bin\Release`로 복사해야 한다(원본 `gvtrademap_cs/README.txt`).
- 테스트 프로젝트·테스트 프레임워크는 없다. 검증은 실제 실행으로만 가능하다.

## 아키텍처

### 실행 흐름

`Program.Main`은 `Application.Run`을 쓰지 않는다. 단일 인스턴스 Mutex → MDX 어셈블리 로드 확인 → `gvtrademap_cs_form.Initialize()` → `while(frm.Created)` 수동 루프에서 `TickCount` 기준 60fps로 `update_main_window()`를 호출하고 `Application.DoEvents()` + `Thread.Sleep(1)`을 돈다. 즉 **모든 갱신·렌더링은 이 한 루프 안에서 일어난다**. 프레임 단위 작업을 추가할 곳은 `do_tasks()`(로직)와 `do_draw()`/`draw_map()`(렌더링)이다.

### 두 개의 중심 객체

- `gvt_lib` (`lib.cs`) — `d3d_device`, `LoopXImage`(지도), `icons`, `nameTexture`, `GlobalSettings`, `IniProfileSetting`, `KeyAssignManager`를 묶은 공용 컨테이너. Dispose 순서가 중요하며 **D3D 디바이스를 가장 마지막에 해제**해야 한다. 키 할당 항목은 `gvt_lib.init_key_assign()`에 `KeyFunction` enum과 1:1로 등록된다 — 기능을 추가하려면 enum과 이 메서드, 그리고 처리부(`process_cnd_key`)를 함께 건드려야 한다.
- `GvoDatabase` (`database/database.cs`) — 도메인 데이터/서비스 집합(`GvoWorldInfo`, `SeaRoutes`, `ShareRoutes`, `WebIcons`, `gvo_chat`, `map_mark`, `ItemDatabaseCustom`, `ShipPartsDataBase`, `sea_area`, `speed_calculator` 등). 생성자에서 대부분의 DB 파일을 읽고, `WriteSettings()`에서 사용자 데이터를 되쓴다.

### UI가 두 겹이다

- `window/` 아래의 `item_window`, `setting_window`, `info_windows`는 WinForms 컨트롤이 **아니라** `library_cs/directx/d3d_window.cs`의 `d3d_windows.window`를 상속한 D3D 자체 렌더링 윈도우다. 좌표 히트 테스트, 최소화, 툴팁까지 직접 구현돼 있다.
- 진짜 WinForms 대화상자는 `gvtrademap_cs/form/`에 있다(`find_form2`, `sea_routes_form2`, `setting_form2`, `map_mark_form` 등). 파일명에 `2`가 붙은 쪽이 현행이고, 접미사 없는 구버전(`setting_form.cs`, `sea_routes_form.cs`)과 `draw_setting_form.cs`, `version_form.cs`, `error_form.cs`는 디스크에만 남아 있을 뿐 csproj의 Compile 목록에서 빠진 잔재다(빌드에 포함되지 않음) — **파일을 고치기 전에 csproj에 실제로 포함돼 있는지 먼저 확인**할 것.

### 좌표계

세 좌표계가 공존하며 변환은 전부 `transform.cs`에만 있다.
- 클라이언트/화면 좌표 ↔ 지도 이미지 픽셀 좌표(`LoopXImage`가 스크롤·확대 담당) ↔ 게임 좌표(`def.GAME_WIDTH` 16384 × `def.GAME_HEIGHT` 8192).
- 지도는 X축으로 순환한다. X 방향 거리 계산에는 반드시 `transform.SubVector_LoopX()`를 써야 하고, `LoopXImage`는 좌우 끝을 이어 그리기 위해 같은 타일을 여러 번 그린다.
- 도시 좌표를 옮기는 작업은 `database/worldinfos.xml`의 `<position>`을 고치는 일이며, 지도 이미지 크기와 무관하게 게임 좌표 비율로 환산된다.

### 스레드

`gvtrademap_cs_form`이 지도 로드/정보 로드/항로 공유/채팅 로그 분석용 백그라운드 스레드를 각각 띄우고 `m_exit_thread_event`(ManualResetEvent)로 종료를 통보한다. `is_load` 중에는 `update_main_window_load()`가 진행률 화면만 그린다. 백그라운드에서 D3D 리소스를 만지지 말 것.

### 게임 클라이언트 연동

- 화면 캡처: `gvo/gvo_capture.cs` → `library_cs/gvo_base/gvo_capture_base.cs`. 게임 창은 **창 제목 없이 클래스명만으로** 찾는다(`gvo_def.GVO_CLASS_NAME = "Greate Voyages Online Game MainFrame"`). 일/한/영문판 공통 동작을 위한 의도적 설계이므로 창 제목 매칭을 다시 넣지 말 것.
- 채팅 로그: `내 문서\KOEI\GV Online\Log\Chat\`을 정규식 규칙 테이블(`gvo_chat_base.analize_data`)로 파싱한다.
- 네트워크: 항로 공유·@web 아이콘은 `def.URL_HP_ORIGINAL`(`http://gvtrademap.daa.jp/`) 대상 HTTP, 그 밖에 `net`/`gvo_net_base`의 TCP 래퍼가 있다. 업데이트 확인은 이 저장소의 `version.dat`(raw.githubusercontent.com, `update_checker`)를 본다. HTTPS 를 위해 `HttpDownload` 정적 생성자에서 TLS 1.2(`(SecurityProtocolType)3072`)를 켠다.
- 외부 링크: 도시·상륙지 클릭은 인벤 지리 DB(`def.INVEN_AREACITY_URL` + `worldinfos.xml` 의 `url_index`), 레시피 검색은 대항해시대 조이(`gvo_def.URL2/URL3`, UTF-8 인코드). 해역 정보 수집(`gvoac.exe`)은 일본판 전용 도구라 기본 단축키 없이 수동 실행만 한다.

### 데이터 파일

`gvtrademap_cs/database/`의 파일들은 출력 폴더로 복사되며 런타임에 읽힌다. 경로 상수는 전부 `def.cs`에 모여 있다.
- `worldinfos.xml` — 도시/해역 마스터(좌표, 이름 오프셋, 각도, 국가, 주점 플래그 등)
- `cityinfos.xml` — 도시별 교역품/가격, `item_db.txt` — CSV(`id,분류,이름,설명`, UTF-8 BOM), `itemtype_db.xml`, `ShipParts.xml`, `webicons.txt`, `domaininfo.xml`
- 데이터 간 이름 일치가 기능의 전제다: `cityinfos.xml`·`itemtype_db.xml`·`ShipParts.xml` 의 아이템명은 `item_db.txt` 의 이름으로 조회된다(`ItemDatabase.Find` — 정확 일치 → `m_ajust_name_list` → 띄어쓰기·괄호·♂♀ 무시 비교). 이름을 고칠 때는 `item_db.txt` 표기를 기준으로 할 것. `item_db.txt` 의 분류명(2열)은 `gvo_item_database.cs` 의 switch 와 1:1이므로 바꾸면 카테고리 판정이 깨진다.
- `ShipParts.xml` 의 `PartsType`/`ShipSizeType` 값은 `ShipPartsDataBase` 의 한국어 키워드("선체","돛","대포","병기" / "소형","중형","대형","전부","중형, 대형")와 정확히 같아야 한다. 해석 실패 시 조용히 기본값(Hull/All)이 된다.
- `cityinfos.xml` 의 그룹명은 `world_info.cs` 의 `m_group_name_tbl` 과 정확히 같아야 한다. 다르면 그 그룹은 통째로 읽히지 않는다.
- `worldinfos.xml` 의 `url_index` 는 인벤 지리 DB 의 code 다(원본의 일본 사이트 번호에서 교체). 인벤에 없는 장소는 `-1`.
- 장소명은 한국 서버 표기를 따른다: 인벤 지리 DB 를 먼저 보고, 없으면 KR v2.0.4 표기를 쓴다. `worldinfos.xml` 과 `cityinfos.xml` 의 이름은 정확히 같아야 하고(다르면 품목이 조용히 버려짐), 같은 이름이 두 번 있으면 안 된다.
- `gvtrademap_cs/データフォーマット.txt`(Shift-JIS) — 구버전 텍스트 포맷 문서지만 **필드 의미(주점 인덱스 비트, 도시 종류, 동맹 종류, 국가 번호)는 현행 XML 속성에 그대로 대응**하므로 데이터 편집 시 참고 문서다.
- 런타임 생성 폴더: `temp\`(항로도·해역), `memo\`, `SS\`, `map\`. 설정은 `gvtrademap_cs.ini`에 `IIniSaveLoad.IniLoad/IniSave` 구현(`GlobalSettings`, `KeyAssignManager`)으로 저장된다.

### library_cs

`directx`(D3D 래퍼, 스프라이트, 폰트, 자체 윈도우 시스템), `utility`(ini, 키 할당, 스크린 캡처, win32 래퍼), `gvo_base`(게임 특화 파싱·DB 베이스), `net`/`gvo_net_base`(TCP), `PanelManager`. **`useful_win32`는 솔루션에 포함되지 않은 레거시**로 `utility`가 대체했다.

`utility/IIni.cs`, `utility/EnumParser*.cs`, `gvo_base/ShipPartsDataBase.cs`는 배포된 exe에서 역컴파일해 복원한 파일이다(헤더에 `Assembly location:` 주석이 남아 있음). 스타일이 다른 것은 의도된 것이 아니라 출처 때문이다.

## 작업 시 주의사항

- **인코딩·서식**: 모든 `.cs`는 UTF-8 **with BOM** + CRLF + 탭 들여쓰기다. 이걸 바꾸면 diff가 파일 전체로 번진다. 데이터 파일도 원래 인코딩(예: `データフォーマット.txt`는 Shift-JIS)을 유지할 것.
- **주석 언어**: 원본 일본어 주석은 한글화가 끝났다(2026-08). 지금 소스에 남아 있는 일본어는 **전부 의도적으로 남긴 것**이므로 번역하지 말 것:
  - (2026-10 정리 완료) `gvo_map_cs_chat_base.cs` 의 `AddAnalizeList()` 패턴, `gvo_chat_base.cs` 의 예금 이자 패턴은 한국어판 게임 문구로, `gvo_item_database.cs` 의 `m_ajust_name_list`·분류 switch 와 `world_info.cs` 의 `update_rename()` 은 한국어 데이터 표기로 교체됐다. 이들은 여전히 **게임 출력·데이터 문자열과 글자 단위로 대조하는 값**이므로 띄어쓰기·문장부호(예: `에게 승리했습니다！`의 전각 느낌표)를 "다듬지" 말 것. 바꿀 때는 실제 게임 로그나 `item_db.txt` 로 확인한다.
  - `library_cs/utility/useful.cs`, `library_cs/directx/d3d_device.cs` 의 `"メイリオ"` — Windows 폰트 이름(Meiryo)이라 바꾸면 폰트를 찾지 못한다.
  - `library_cs/utility/SFMT.cs`, `RandomBase.cs` — 제3자 라이브러리(Copyright © Rei HOBARA 2007), Shift-JIS 인코딩. 손대지 말 것.
- **네이밍**: 필드는 `m_` 접두사, 메서드·지역변수는 snake_case가 다수(일본어 원본 스타일). 새 코드도 주변 파일 스타일을 따를 것.
- **셰이더 미지원 폴백**: `d3d_sprite`의 `m_effect`가 `null`일 수 있고, `draw_infonames.cs` 등 여러 곳이 `if (effect == null) return;`로 우회한다. 렌더링 코드를 추가할 때 이 경로를 깨뜨리지 말 것.
- **버전 문자열**: 릴리스 시 `def.VERSION`(정수)과 `def.WINDOW_TITLE`, 저장소 루트 `version.dat`(1행째 = 새 VERSION, 2행째 이후 = 업데이트 내용)을 함께 갱신한다. `version.dat` 은 master 에 올라가야 사용자에게 보인다.
- **알려진 quirk**: `gvtrademap_cs.csproj`의 `utility` ProjectReference GUID가 `{154900C4-...}`(= `useful_win32`의 GUID)로 잘못 적혀 있다. 실제 `utility.csproj`는 `{793B3A37-...}`다. 경로 기준으로는 정상 빌드되므로 굳이 손대지 말고, "고친다"며 `useful_win32`를 참조하게 만들지 말 것.
- **데이터 커밋**: `database/` 갱신은 커뮤니티 기여자(백지훈, Seyren, 데이셔넬 등)가 제공한 자료를 반영하는 작업이다. 커밋 메시지에 출처를 남기는 관례가 있고, 스키마는 유지한 채 값만 갱신한다. 2026-10 의 일본어 잔재 정리는 KR v2.0.4 배포판(연이v)의 같은 ID 행과 의미 대조, 인벤 대항해시대 DB(dho.inven.co.kr — robots 허용, 요청 간격을 둘 것), 직접 번역을 출처로 했다. 대항해시대 조이(ssjoy.org)는 robots.txt 가 전체 금지이고 봇 차단이 있어 자동 수집하지 않는다.
  - 일본 쪽 출처: 원본 이후 갱신된 유일한 포크는 `bN2DhXKD/gvtrademap_cs`(2023-06~2024-05, 일본 서버 월드 통합 대응). 신규 장소·풍향만 가져왔다(좌표는 지도 픽셀이라 공통 도시로 게임 좌표 환산). 아이템·도시별 교역품은 정리 wiki(gvo.gamedb.info — robots 허용, **Crawl-delay 60**)를 쓴다. gvdb.mydns.jp 는 robots 가 일반 봇을 막으므로 자동 수집하지 않는다.
  - `item_db.txt` 의 ID `9000001~` 은 참고판에 대응 행이 없어 일본 wiki 기준으로 직접 번역해 넣은 행이다(앱은 ID 를 쓰지 않는다). 참고판과 같은 아이템을 찾으면 그 행(ID·표기)으로 바꾸면 된다.
