/*-------------------------------------------------------------------------

 설정항목

---------------------------------------------------------------------------*/

/*-------------------------------------------------------------------------
 using
---------------------------------------------------------------------------*/
using System;
using System.Drawing;

using Utility;
using Utility.Ini;

/*-------------------------------------------------------------------------
 
---------------------------------------------------------------------------*/
namespace gvtrademap_cs
{
	// 지도
	public enum MapIndex{
		Map1,				// 맵 1번
		Map2,				// 맵 2번
		Max
	};
	public enum MapIcon{
		Big,				// 대きい아이콘
		Small,              // 소さい아이콘
        CityAndShore,       // 도시와 1차필드만
		OnlyBigCity,		// 대도시 (소아이콘)
        Hide,
	};
	public enum MapDrawNames{
		Draw,			   // 도시명, 해역명, 전체 상륙지명 그리기
        CityAndShore,      // 도시와 해역명, 1차필드까지만
		OnlyCity,          // 도시와 해역명 그리기
		OnlyBigCity,       // 조합도시 그리기
		Hide,			   // 명칭을 그리지 않음
	};
	// 화면캡처간격
	public enum CaptureIntervalIndex{
		Per500ms,		// 0.5초에1회
		Per1000ms,		// 1초에1회
		Per2000ms,		// 2초에1회
		Per250ms,		// 0.25초에1회
	};
	// 위도, 경도그리기간격
	public enum TudeInterval{
		None,			// なし
		Interval1000,	// 1000刻み
		Interval100,	// 100刻み
		OnlyPoints,		// 좌표値のみ
	};
	// 검색フィルタ
	public enum _find_filter{
		world_info,			// 도시정보からのみ
		item_database,		// 아이템DBからのみ
		both,				// 両方
	};
	// 검색フィルタ2
	public enum _find_filter2{
		name,				// 명칭 등から
		type,				// 종류별로
	};
	// 검색フィルタ3
	public enum _find_filter3{
		full_text_search,	// 全文검색(부분일치)
		full_match,			// 완전일치
		prefix_search,		// 앞부분일치
		suffix_search,		// 뒷부분일치
	};

	// 표시항목ページ
	public enum DrawSettingPage{
		WebIcons,		// @Web아이콘
		MemoIcons,		// 메모아이콘
		Accidents,		// 재해
		MyShipAngle,	// 예상선
	};
	// @Web icons
	[Flags]
	public enum DrawSettingWebIcons{
		wind				= 1<<0,
		accident_0			= 1<<1,
		accident_1			= 1<<2,
		accident_2			= 1<<3,
		accident_3			= 1<<4,
		accident_4			= 1<<5,
	};
	// Memo icons
	[Flags]
	public enum DrawSettingMemoIcons{
		wind				= 1<<0,
		memo_0				= 1<<1,
		memo_1				= 1<<2,
		memo_2				= 1<<3,
		memo_3				= 1<<4,
		memo_4				= 1<<5,
		memo_5				= 1<<6,
		memo_6				= 1<<7,
		memo_7				= 1<<8,
		memo_8				= 1<<9,
		memo_9				= 1<<10,
		memo_10				= 1<<11,
		memo_11				= 1<<12,
	};
	// accidents
	[Flags]
	public enum DrawSettingAccidents{
		accident_0			= 1<<0,
		accident_1			= 1<<1,
		accident_2			= 1<<2,
		accident_3			= 1<<3,
		accident_4			= 1<<4,
		accident_5			= 1<<5,
		accident_6			= 1<<6,
		accident_7			= 1<<7,
		accident_8			= 1<<8,
		accident_9			= 1<<9,
		accident_10			= 1<<10,
	};
	// myship_angle
	[Flags]
	public enum DrawSettingMyShipAngle{
		draw_0				= 1<<0,
		draw_1				= 1<<1,
	};
	public enum SSFormat{
		Bmp,
		Png,
		Jpeg,
	};
	
	/*-------------------------------------------------------------------------
	 
	---------------------------------------------------------------------------*/
	public class GlobalSettings : IIniSaveLoad
	{
		// 항로도の保持수の初期値
		private const int				DEF_SEAROUTES_GROUP_MAX			= 20;
		private const int				DEF_TRASH_SEAROUTES_GROUP_MAX	= 200;

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		// 윈도우위치と사이즈
		private Point					m_window_location;			// 윈도우위치
		private Size					m_window_size;				// 윈도우사이즈

		private Point					m_find_window_location;		// 검색윈도우위치
		private Size					m_find_window_size;			// 검색윈도우사이즈
		private bool					m_find_window_visible;		// 표시중のときtrue
	
		private Point					m_sea_routes_window_location;	// 항로목록윈도우위치
		private Size					m_sea_routes_window_size;		// 항로목록윈도우사이즈
		private bool					m_sea_routes_window_visible;	// 항로목록표시중のときtrue

		private	bool					m_save_searoutes;			// 항로기록
		private bool					m_draw_share_routes;		// 항로공유
		private bool					m_draw_icons;				// 메모아이콘그리기
		private bool					m_draw_sea_routes;			// 항로도그리기
		private int						m_draw_popup_day_interval;	// 말풍선표시간격
		private bool					m_draw_accident;			// 재해그리기
		private bool					m_center_myship;			// 본인の배중心に그리기
		private bool					m_draw_myship_angle;		// 예상선그리기
		private bool					m_draw_web_icons;			// @Web아이콘그리기

		// ダイア로그설정항목
		private GvoWorldInfo.Server			m_server;					// 서버
		private GvoWorldInfo.Country			m_country;					// 자국
		private MapIndex				m_map_index;				// 지도
		private MapIcon				m_map_icon;					// 지도の아이콘사이즈
		private MapDrawNames			m_map_draw_names;			// 도시명などを그리기するかどうか
		private string					m_share_group;				// 항로공유그룹명
		private string					m_share_group_myname;		// 항로공유표시명
		private int						m_searoutes_group_max;		// 覚えておく항로도の최대
		private int						m_trash_searoutes_group_max;	// 覚えておくごみ箱항로도の최대
		private bool					m_connect_network;			// ネットワークに연결する
		private bool					m_hook_mouse;				// 마우스の進む/戻るボタンで스킬/도구窓を開く
		private bool					m_windows_vista_aero;		// Aero모드
		private bool					m_enable_analize_log_chat;	// 재해용に로그を분석する
		private bool					m_is_share_routes;			// 항로공유する
		private CaptureIntervalIndex	m_capture_interval;			// 화면캡처간격
		private bool					m_connect_web_icon;			// @Web아이콘をダウンロードする
		private bool					m_compatible_windows_rclick;	// 우클릭の動作をwindows互換にする
		private TudeInterval			m_tude_interval;			// 위도, 경도그리기방법
		private bool					m_use_mixed_map;			// 항로도즐겨찾기と合成された지도を사용する
		private bool					m_window_top_most;			// 윈도우を常に最前面に표시する
		private bool					m_enable_line_antialias;	// 선の그리기の기자기자を경減する
		private bool					m_enable_sea_routes_aplha;	// アクティブな항로도以외を반투명で그리기する
		private SSFormat				m_ss_format;				// 스크린샷の書き出しフォーマット
		private bool					m_remove_near_web_icons;	// 距離が近く同じ종류の@Web아이콘を목록から삭제する
		private bool					m_draw_capture_info;		// 캡처の디테일を그리기する
		private bool					m_is_server_mode;			// TCP서버모드
		private int						m_port_index;				// 포트번호
		private bool					m_is_mixed_info_names;		// 도시명を지도に合成する
		private int						m_minimum_draw_days;		// 그리기する최저항해일수
		private bool					m_enable_favorite_sea_routes_alpha;	// 즐겨찾기항로도を반투명で그리기する
		private bool					m_draw_favorite_sea_routes_alpha_popup;	// 즐겨찾기항로도の재해ポップアップを그리기する
		private	bool					m_debug_flag_show_potision;	// 정보윈도우の좌표を지도좌표계で표시する
		private bool					m_enable_dpi_scaling;	   // DPIスケーリングに대응するフリをする

		// 표시関係
		private string					m_select_info;				// 선택중の세계정보
		private float					m_map_pos_x;				// 지도の그리기オフセット
		private float					m_map_pos_y;				// 지도の그리기オフセット
		private float					m_map_scale;				// 지도の스케일

		// 윈도우상태
		private bool					m_is_item_window_normal_size;	// 통상상태のときtrue
		private bool					m_is_setting_window_normal_size;	// 통상상태のときtrue

		private bool					m_is_border_style_none;		// 윈도우枠の표시なしのときtrue

		private UniqueString			m_find_strings;				// 검색履歴
		private _find_filter			m_find_filter;				// 검색フィルタ
		private _find_filter2			m_find_filter2;				// 검색フィルタ2
		private _find_filter3			m_find_filter3;				// 검색フィルタ3

		// 이자からの경과일수
		private int						m_interest_days;

		// 조선からの경과일수
		private bool					m_force_show_build_ship;	// 조선중でなくても윈도우を표시する
		private bool					m_is_now_build_ship;		// 조선중ならtrue
		private string					m_build_ship_name;			// 선박명
		private int						m_build_ship_days;			// 조선중일수


		// 표시항목
		private DrawSettingWebIcons		m_draw_setting_web_icons;
		private DrawSettingMemoIcons		m_draw_setting_memo_icons;
		private DrawSettingAccidents		m_draw_setting_accidents;
		private DrawSettingMyShipAngle	m_draw_setting_myship_angle;
		private bool						m_draw_setting_myship_angle_with_speed_pos;
		private bool						m_draw_setting_myship_expect_pos;
		
		// リクエスト
		private RequestCtrl			m_req_screen_shot;			// 스크린샷
		private RequestCtrl			m_req_update_map;			// 지도の合成リクエスト

		private RequestCtrl			m_req_centering_gpos;		// 特定の게임좌표をセンタリングするリクエスト
		private Point					m_centering_gpos;			// センタリングする좌표

		private RequestCtrl			m_req_spot_item;			// 장소リクエスト
		private RequestCtrl			m_req_spot_item_changed;	// 장소リクエスト(대상변경)

		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public Point window_location{			get{	return m_window_location;			}
												set{	m_window_location	= value;		}}
		public Size window_size{				get{	return m_window_size;				}
												set{	m_window_size		= value;		}}
		public Point find_window_location{		get{	return m_find_window_location;		}
												set{	m_find_window_location	= value;	}}
		public Size find_window_size{			get{	return m_find_window_size;			}
												set{	m_find_window_size		= value;	}}
		public bool find_window_visible{		get{	return m_find_window_visible;		}
												set{	m_find_window_visible	= value;	}}

		public Point sea_routes_window_location{	get{	return m_sea_routes_window_location;	}
													set{	m_sea_routes_window_location	= value;	}}
		public Size sea_routes_window_size{		get{	return m_sea_routes_window_size;		}
												set{	m_sea_routes_window_size	= value;	}}
		public bool sea_routes_window_visible{	get{	return m_sea_routes_window_visible;		}
												set{	m_sea_routes_window_visible	= value;	}}
	
		public bool save_searoutes{				get{	return m_save_searoutes;			}
												set{	m_save_searoutes	= value;		}}
		public bool	draw_share_routes{			get{	return m_draw_share_routes;				}
												set{	m_draw_share_routes		= value;		}}
		public bool	draw_icons{					get{	return m_draw_icons;				}
												set{	m_draw_icons		= value;		}}
		public bool	draw_sea_routes{			get{	return m_draw_sea_routes;			}
												set{	m_draw_sea_routes	= value;		}}
		public int	draw_popup_day_interval{	get{	return m_draw_popup_day_interval;	}
												set{
													m_draw_popup_day_interval	= value;
													switch(m_draw_popup_day_interval){
													case 1:
													case 5:
														break;
													default:
														m_draw_popup_day_interval	= 0;	// 표시なし
														break;
													}
												}
											}
		public bool	draw_accident{				get{	return m_draw_accident;					}
												set{	m_draw_accident		= value;			}}
		public bool	center_myship{				get{	return m_center_myship;					}
												set{	m_center_myship		= value;			}}
		public bool	draw_myship_angle{			get{	return m_draw_myship_angle;				}
												set{	m_draw_myship_angle	= value;			}}
		
		public MapIndex	map{				get{	return m_map_index;						}
												set{	m_map_index	= value;					}}
		public MapIcon	map_icon{				get{	return m_map_icon;						}
												set{	m_map_icon	= value;					}}
		public MapDrawNames map_draw_names{	get{	return m_map_draw_names;				}
												set{	m_map_draw_names	= value;			}}
		public GvoWorldInfo.Server server{			get{	return m_server;						}
												set{	m_server	= value;					}}
		public GvoWorldInfo.Country country{			get{	return m_country;						}
												set{	m_country	= value;					}}
	

		public string share_group{				get{	return m_share_group;					}
												set{	m_share_group	= value;				}}
		public string share_group_myname{		get{	return m_share_group_myname;			}
												set{	m_share_group_myname	= value;		}}
	

		public string select_info{				get{	return m_select_info;					}
												set{	m_select_info	= value;				}}
		public float map_pos_x{					get{	return m_map_pos_x;						}
												set{	m_map_pos_x		= value;				}}
		public float map_pos_y{					get{	return m_map_pos_y;						}
												set{	m_map_pos_y		= value;				}}
		public float map_scale{					get{	return m_map_scale;						}
												set{	m_map_scale		= value;				}}
		public UniqueString find_strings{		get{	return m_find_strings;					}}
		public _find_filter find_filter{		get{	return m_find_filter;					}
												set{	m_find_filter	= value;				}}
		public _find_filter2 find_filter2{		get{	return m_find_filter2;					}
												set{	m_find_filter2	= value;				}}
		public _find_filter3 find_filter3{		get{	return m_find_filter3;					}
												set{	m_find_filter3	= value;				}}
		public int searoutes_group_max{			get{	return m_searoutes_group_max;			}
												set{
													m_searoutes_group_max		= value;
													if(m_searoutes_group_max <= 0)		m_searoutes_group_max		= def_searoutes_group_max;
												}
							}
		public int def_searoutes_group_max{				get{	return DEF_SEAROUTES_GROUP_MAX;		}}
		public int trash_searoutes_group_max{	get{	return m_trash_searoutes_group_max;		}
												set{
													m_trash_searoutes_group_max	= value;
													if(m_trash_searoutes_group_max <= 0)	m_trash_searoutes_group_max	= def_trash_searoutes_group_max;
												}
							}
		public int def_trash_searoutes_group_max{		get{	return DEF_TRASH_SEAROUTES_GROUP_MAX;		}}


		public RequestCtrl req_screen_shot{			get{	return m_req_screen_shot;			}}
		public RequestCtrl req_update_map{				get{	return m_req_update_map;			}}

		public bool	connect_network{			get{	return m_connect_network;				}
												set{	m_connect_network		= value;		}}
		public bool hook_mouse{					get{	return m_hook_mouse;					}
												set{	m_hook_mouse			= value;		}}
		public bool windows_vista_aero{			get{	return m_windows_vista_aero;			}
												set{	m_windows_vista_aero	= value;		}}
		public bool enable_analize_log_chat{	get{	return m_enable_analize_log_chat;		}
												set{	m_enable_analize_log_chat	= value;	}}

		public bool is_share_routes{			get{	return m_is_share_routes;				}
												set{	m_is_share_routes		= value;		}}
		public SSFormat ss_format			{	get{	return m_ss_format;						}
												set{	m_ss_format	= value;					}}

		// 외부からの항로공유유효かどうかのチェック용
		public bool enable_share_routes{
			get{
				if(!connect_network)			return false;		// インターネットに연결しない
				if(!is_share_routes)			return false;		// 공유무효
				if(share_group == "")			return false;		// 그룹명が空欄
				if(share_group_myname == "")	return false;		// 본인の배の이름が空欄
				return true;		// 유효
			}
		}

		public int interest_days{				get{	return m_interest_days;					}
												set{	m_interest_days		= value;			}}

		public bool force_show_build_ship{		get{	return m_force_show_build_ship;			}
												set{	m_force_show_build_ship	= value;		}}
		public bool is_now_build_ship{			get{	return m_is_now_build_ship;				}
												set{	m_is_now_build_ship		= value;		}}
		public string build_ship_name{			get{	return m_build_ship_name;				}
												set{	m_build_ship_name		= value;		}}
		public int build_ship_days{				get{	return m_build_ship_days;				}
												set{	m_build_ship_days		= value;		}}

		public CaptureIntervalIndex capture_interval{	get{	return m_capture_interval;			}
													set{	m_capture_interval	= value;		}}

		public bool connect_web_icon{			get{	return m_connect_web_icon;				}
												set{	m_connect_web_icon	= value;			}}
		// @Web아이콘を로드かどうかを得る
		public bool is_load_web_icon{			get{	if(!connect_network)	return false;
														return connect_web_icon;				}}

		public bool compatible_windows_rclick{	get{	return m_compatible_windows_rclick;		}
												set{	m_compatible_windows_rclick	= value;	}}

		public bool is_item_window_normal_size{	get{	return m_is_item_window_normal_size;				}
												set{	m_is_item_window_normal_size	= value;			}}
		public bool is_setting_window_normal_size{	get{	return m_is_setting_window_normal_size;		}
													set{	m_is_setting_window_normal_size	= value;	}}
		public bool is_border_style_none{		get{	return m_is_border_style_none;			}
												set{	m_is_border_style_none	= value;		}}

		public TudeInterval tude_interval{		get{	return m_tude_interval;					}
												set{	m_tude_interval		= value;			}}
		public bool draw_web_icons{				get{	return m_draw_web_icons;				}
												set{	m_draw_web_icons	= value;			}}
		public bool use_mixed_map{				get{	return m_use_mixed_map;					}
												set{	m_use_mixed_map		= value;			}}

		public RequestCtrl req_centering_gpos{	get{	return m_req_centering_gpos;			}}
		public Point centering_gpos{			get{	return m_centering_gpos;				}
												set{	m_centering_gpos	= value;			}}

		public RequestCtrl req_spot_item{		get{	return m_req_spot_item;					}}
		public RequestCtrl req_spot_item_changed{	get{	return m_req_spot_item_changed;		}}

		public bool window_top_most{			get{	return m_window_top_most;				}
												set{	m_window_top_most	= value;			}}
	
		public bool enable_line_antialias{		get{	return m_enable_line_antialias;			}
												set{	m_enable_line_antialias	= value;		}}
		public bool enable_sea_routes_aplha{	get{	return m_enable_sea_routes_aplha;		}
												set{	m_enable_sea_routes_aplha	= value;	}}
		public bool remove_near_web_icons{		get{	return m_remove_near_web_icons;			}
												set{	m_remove_near_web_icons		= value;	}}
		public bool draw_capture_info{			get{	return m_draw_capture_info;				}
												set{	m_draw_capture_info			= value;	}}
		public bool is_server_mode{				get{	return m_is_server_mode;				}
												set{	m_is_server_mode			= value;	}}
		public int port_index{					get{	return m_port_index;					}
												set{	m_port_index				= value;	}}
		public bool is_mixed_info_names{		get{	return m_is_mixed_info_names;			}
												set{	m_is_mixed_info_names		= value;	}}
		public int minimum_draw_days{			get{	return m_minimum_draw_days;				}
												set{
													m_minimum_draw_days			= value;
													if(m_minimum_draw_days < 0)	m_minimum_draw_days	= 0;
												}
									}
		public bool enable_favorite_sea_routes_alpha		{	get{	return m_enable_favorite_sea_routes_alpha;			}
																set{	m_enable_favorite_sea_routes_alpha		= value;	}}
		public bool draw_favorite_sea_routes_alpha_popup	{	get{	return m_draw_favorite_sea_routes_alpha_popup;		}
																set{	m_draw_favorite_sea_routes_alpha_popup	= value;	}}
		public bool debug_flag_show_potision				{	get{	return m_debug_flag_show_potision;		}
																set{	m_debug_flag_show_potision	= value;	}}
		public bool enable_dpi_scaling					  {   get{	return m_enable_dpi_scaling;  }
																set{	m_enable_dpi_scaling = value; }}

		// 표시항목
		public DrawSettingWebIcons draw_setting_web_icons{		get{	return m_draw_setting_web_icons;	}
																	set{	m_draw_setting_web_icons	= value;	}}
		public DrawSettingMemoIcons draw_setting_memo_icons{		get{	return m_draw_setting_memo_icons;	}
																	set{	m_draw_setting_memo_icons	= value;	}}
		public DrawSettingAccidents draw_setting_accidents{		get{	return m_draw_setting_accidents;	}
																	set{	m_draw_setting_accidents	= value;	}}
		public DrawSettingMyShipAngle draw_setting_myship_angle{	get{	return m_draw_setting_myship_angle;	}
																	set{	m_draw_setting_myship_angle	= value;	}}
		public bool draw_setting_myship_angle_with_speed_pos{		get{	return m_draw_setting_myship_angle_with_speed_pos;	}
																	set{	m_draw_setting_myship_angle_with_speed_pos	= value;	}}
		public bool draw_setting_myship_expect_pos{					get{	return m_draw_setting_myship_expect_pos;		}
																	set{	m_draw_setting_myship_expect_pos	= value;	}}

		// 未사용
		public string DefaultIniGroupName			{				get{	return "GlobalSettings";		}}


		/*-------------------------------------------------------------------------
		 
		---------------------------------------------------------------------------*/
		public GlobalSettings()
		{
			// 初期値설정
			init();
		}

		/*-------------------------------------------------------------------------
		 リクエストを전부취소する
		---------------------------------------------------------------------------*/
		public void CancelAllRequests()
		{
			req_screen_shot.CancelRequest();
			req_update_map.CancelRequest();
			req_centering_gpos.CancelRequest();
			req_spot_item.CancelRequest();
			req_spot_item_changed.CancelRequest();
		}
	
		/*-------------------------------------------------------------------------
		 初期値を설정する
		---------------------------------------------------------------------------*/
		private void init()
		{
			// 윈도우위치と사이즈
			window_location				= new Point(10, 10);
			window_size					= new Size(640, 400);
			// 검색윈도우위치と사이즈
			find_window_size			= new Size(468, 320);
			find_window_location		= new Point(window_location.X + ((window_size.Width / 2) - (find_window_size.Width / 2)),
													window_location.Y + ((window_size.Height / 2) - (find_window_size.Height / 2)));
			find_window_visible			= false;
				
			// 항로도목록윈도우위치と사이즈
			sea_routes_window_size		= new Size(690, 320);
			sea_routes_window_location	= new Point(window_location.X + ((window_size.Width / 2) - (sea_routes_window_size.Width / 2)),
													window_location.Y + ((window_size.Height / 2) - (sea_routes_window_size.Height / 2)));
			sea_routes_window_visible	= false;
	
			save_searoutes				= true;
			draw_share_routes			= false;
			draw_web_icons				= false;
			draw_icons					= false;
			draw_sea_routes				= true;
			draw_popup_day_interval		= 5;		// 5일
			draw_accident				= false;
			center_myship				= false;
			draw_myship_angle			= true;

			// ダイア로그설정항목
			server						= GvoWorldInfo.Server.Euros;
			country						= GvoWorldInfo.Country.England;
			m_map_index					= MapIndex.Map1;
			m_map_icon					= MapIcon.Big;
			m_map_draw_names			= MapDrawNames.Draw;
			share_group					= "";
			share_group_myname			= "";
			searoutes_group_max			= DEF_SEAROUTES_GROUP_MAX;
			trash_searoutes_group_max	= DEF_TRASH_SEAROUTES_GROUP_MAX;
			connect_network				= true;
			hook_mouse					= false;
			windows_vista_aero			= false;
			enable_analize_log_chat		= true;
			is_share_routes				= false;
			capture_interval			= CaptureIntervalIndex.Per1000ms;
			connect_web_icon			= false;
			compatible_windows_rclick	= false;
			this.tude_interval			= TudeInterval.OnlyPoints;
			use_mixed_map				= true;
			window_top_most				= false;
			enable_line_antialias		= true;
			enable_sea_routes_aplha		= false;
			m_ss_format					= SSFormat.Bmp;
			remove_near_web_icons		= true;
			draw_capture_info			= false;
			is_server_mode				= false;
			port_index					= def.DEFALUT_PORT_INDEX;
			is_mixed_info_names			= false;
			minimum_draw_days			= 0;
			enable_favorite_sea_routes_alpha		= true;
			draw_favorite_sea_routes_alpha_popup	= false;
			debug_flag_show_potision	= false;
			enable_dpi_scaling		  = false;

			// 표시関係
			select_info					= "";		// 선택なし
			map_pos_x					= 0;
			map_pos_y					= 0;
			map_scale					= 1;

			// 윈도우상태
			is_item_window_normal_size		= true;
			is_setting_window_normal_size	= true;

			// 윈도우枠の표시/비표시
			is_border_style_none		= false;

			// 검색履歴
			m_find_strings				= new UniqueString();
			find_filter					= _find_filter.both;
			find_filter2				= _find_filter2.name;
			find_filter3				= _find_filter3.full_text_search;

			// 이자からの경과일수
			this.interest_days			= 0;
	
			// 조선からの경과일수
			this.force_show_build_ship	= false;
			this.is_now_build_ship		= false;
			this.build_ship_name		= "";
			this.build_ship_days		= 0;
	
			// リクエスト
			m_req_screen_shot			= new RequestCtrl();
			m_req_update_map			= new RequestCtrl();
			m_req_centering_gpos		= new RequestCtrl();
			m_req_spot_item				= new RequestCtrl();
			m_req_spot_item_changed		= new RequestCtrl();

			m_centering_gpos			= new Point(-1, -1);

			// 표시항목	
			this.draw_setting_web_icons		= DrawSettingWebIcons.wind
											| DrawSettingWebIcons.accident_0
											| DrawSettingWebIcons.accident_1
											| DrawSettingWebIcons.accident_2
											| DrawSettingWebIcons.accident_3
											| DrawSettingWebIcons.accident_4;
			this.draw_setting_memo_icons	= DrawSettingMemoIcons.wind
											| DrawSettingMemoIcons.memo_0
											| DrawSettingMemoIcons.memo_1
											| DrawSettingMemoIcons.memo_2
											| DrawSettingMemoIcons.memo_3
											| DrawSettingMemoIcons.memo_4
											| DrawSettingMemoIcons.memo_5
											| DrawSettingMemoIcons.memo_6
											| DrawSettingMemoIcons.memo_7
											| DrawSettingMemoIcons.memo_8
											| DrawSettingMemoIcons.memo_9
											| DrawSettingMemoIcons.memo_10
											| DrawSettingMemoIcons.memo_11;
			this.draw_setting_accidents		= DrawSettingAccidents.accident_0
											| DrawSettingAccidents.accident_1
											| DrawSettingAccidents.accident_2
											| DrawSettingAccidents.accident_3
											| DrawSettingAccidents.accident_4
											| DrawSettingAccidents.accident_5
											| DrawSettingAccidents.accident_6
											| DrawSettingAccidents.accident_7
											| DrawSettingAccidents.accident_8
											| DrawSettingAccidents.accident_9
											| DrawSettingAccidents.accident_10;
			this.draw_setting_myship_angle	= DrawSettingMyShipAngle.draw_0
											| DrawSettingMyShipAngle.draw_1;
			draw_setting_myship_angle_with_speed_pos	= true;
			draw_setting_myship_expect_pos				= true;
		}
	
		/*-------------------------------------------------------------------------
		 クローン
		---------------------------------------------------------------------------*/
		public GlobalSettings Clone()
		{
			GlobalSettings	s	= new GlobalSettings();

			s.window_location			= window_location;
			s.window_size				= window_size;
			s.find_window_location		= find_window_location;
			s.find_window_size			= find_window_size;
			s.find_window_visible		= find_window_visible;
			s.sea_routes_window_size		= sea_routes_window_size;
			s.sea_routes_window_location	= sea_routes_window_location;
			s.sea_routes_window_visible		= sea_routes_window_visible;

			s.save_searoutes			= save_searoutes;
			s.draw_share_routes			= draw_share_routes;
			s.draw_icons				= draw_icons;
			s.draw_sea_routes			= draw_sea_routes;
			s.draw_popup_day_interval	= draw_popup_day_interval;
			s.draw_accident				= draw_accident;
			s.center_myship				= center_myship;
			s.draw_myship_angle			= draw_myship_angle;

			s.server					= server;
			s.country					= country;
			s.map						= map;
			s.map_icon					= map_icon;
			s.map_draw_names			= map_draw_names;

			s.share_group				= share_group;
			s.share_group_myname		= share_group_myname;

			s.select_info				= select_info;
			s.map_pos_x					= map_pos_x;
			s.map_pos_y					= map_pos_y;
			s.map_scale					= map_scale;

			s.searoutes_group_max		= searoutes_group_max;
			s.trash_searoutes_group_max	= trash_searoutes_group_max;

			// 複製する
			s.m_find_strings.Clone(m_find_strings);
			s.find_filter				= find_filter;
			s.find_filter2				= find_filter2;
			s.find_filter3				= find_filter3;

			s.connect_network			= connect_network;
			s.hook_mouse				= hook_mouse;
			s.windows_vista_aero		= windows_vista_aero;
			s.enable_analize_log_chat	= enable_analize_log_chat;
			s.is_share_routes			= is_share_routes;

			s.interest_days				= this.interest_days;
			s.force_show_build_ship		= this.force_show_build_ship;
			s.is_now_build_ship			= this.is_now_build_ship;
			s.build_ship_name			= this.build_ship_name;
			s.build_ship_days			= this.build_ship_days;

			s.capture_interval			= capture_interval;
			s.connect_web_icon			= connect_web_icon;
			s.compatible_windows_rclick	= compatible_windows_rclick;

			s.is_item_window_normal_size	= is_item_window_normal_size;
			s.is_setting_window_normal_size	= is_setting_window_normal_size;
			s.is_border_style_none		= is_border_style_none;
			s.tude_interval				= this.tude_interval;
			s.draw_web_icons			= draw_web_icons;
			s.use_mixed_map				= use_mixed_map;
			s.window_top_most			= window_top_most;
			s.enable_line_antialias		= enable_line_antialias;
			s.enable_sea_routes_aplha	= enable_sea_routes_aplha;

			s.draw_setting_web_icons	= this.draw_setting_web_icons;
			s.draw_setting_memo_icons	= this.draw_setting_memo_icons;
			s.draw_setting_accidents	= this.draw_setting_accidents;
			s.draw_setting_myship_angle	= this.draw_setting_myship_angle;
			s.draw_setting_myship_angle_with_speed_pos	= this.draw_setting_myship_angle_with_speed_pos;
			s.draw_setting_myship_expect_pos	= this.draw_setting_myship_expect_pos;
			s.ss_format					= this.ss_format;
			s.remove_near_web_icons		= this.remove_near_web_icons;
			s.draw_capture_info			= this.draw_capture_info;
			s.is_server_mode			= this.is_server_mode;
			s.port_index				= this.port_index;
			s.is_mixed_info_names		= this.is_mixed_info_names;
			s.minimum_draw_days			= this.minimum_draw_days;
			s.enable_favorite_sea_routes_alpha		= enable_favorite_sea_routes_alpha;
			s.draw_favorite_sea_routes_alpha_popup	= draw_favorite_sea_routes_alpha_popup;
			s.debug_flag_show_potision	= debug_flag_show_potision;
			s.enable_dpi_scaling		= enable_dpi_scaling;

			// リクエストは전부취소
			s.CancelAllRequests();
			return s;
		}
		// 지도と도시명合成チェック付き
		public void Clone(GlobalSettings s)
		{
			bool	req		= false;
			if((this.map != s.map)
				||(this.map_icon != s.map_icon)
				||(this.map_draw_names != s.map_draw_names)
				||(this.is_mixed_info_names != s.is_mixed_info_names) ){
				// 合成し直す必要あり
				req		= true;
			}
	
			window_location				= s.window_location;
			window_size					= s.window_size;
			find_window_location		= s.find_window_location;
			find_window_size			= s.find_window_size;
			find_window_visible			= s.find_window_visible;
			sea_routes_window_size		= s.sea_routes_window_size;
			sea_routes_window_location	= s.sea_routes_window_location;
			sea_routes_window_visible	= s.sea_routes_window_visible;

			save_searoutes				= s.save_searoutes;
			draw_share_routes			= s.draw_share_routes;
			draw_icons					= s.draw_icons;
			draw_sea_routes				= s.draw_sea_routes;
			draw_popup_day_interval		= s.draw_popup_day_interval;
			draw_accident				= s.draw_accident;
			center_myship				= s.center_myship;
			draw_myship_angle			= s.draw_myship_angle;

			server						= s.server;
			country						= s.country;
			map							= s.map;
			map_icon					= s.map_icon;
			map_draw_names				= s.map_draw_names;
			share_group					= s.share_group;
			share_group_myname			= s.share_group_myname;

			select_info					= s.select_info;
			map_pos_x					= s.map_pos_x;
			map_pos_y					= s.map_pos_y;
			map_scale					= s.map_scale;

			searoutes_group_max			= s.searoutes_group_max;
			trash_searoutes_group_max	= s.trash_searoutes_group_max;

			// 複製する
			m_find_strings.Clone(s.m_find_strings);
			find_filter					= s.find_filter;
			find_filter2				= s.find_filter2;
			find_filter3				= s.find_filter3;

			connect_network				= s.connect_network;
			hook_mouse					= s.hook_mouse;
			windows_vista_aero			= s.windows_vista_aero;
			enable_analize_log_chat		= s.enable_analize_log_chat;
			is_share_routes				= s.is_share_routes;

			this.interest_days			= s.interest_days;
			this.force_show_build_ship	= s.force_show_build_ship;
			this.is_now_build_ship		= s.is_now_build_ship;
			this.build_ship_name		= s.build_ship_name;
			this.build_ship_days		= s.build_ship_days;
	
			capture_interval			= s.capture_interval;
			connect_web_icon			= s.connect_web_icon;
			compatible_windows_rclick	= s.compatible_windows_rclick;

			is_item_window_normal_size	= s.is_item_window_normal_size;
			is_setting_window_normal_size	= s.is_setting_window_normal_size;
			is_border_style_none		= s.is_border_style_none;
			this.tude_interval			= s.tude_interval;
			draw_web_icons				= s.draw_web_icons;
			use_mixed_map				= s.use_mixed_map;
			window_top_most				= s.window_top_most;
			enable_line_antialias		= s.enable_line_antialias;
			enable_sea_routes_aplha		= s.enable_sea_routes_aplha;

			this.draw_setting_web_icons		= s.draw_setting_web_icons;
			this.draw_setting_memo_icons	= s.draw_setting_memo_icons;
			this.draw_setting_accidents		= s.draw_setting_accidents;
			this.draw_setting_myship_angle	= s.draw_setting_myship_angle;
			this.draw_setting_myship_angle_with_speed_pos	= s.draw_setting_myship_angle_with_speed_pos;
			this.draw_setting_myship_expect_pos	= s.draw_setting_myship_expect_pos;
			this.ss_format					= s.ss_format;
			this.remove_near_web_icons		= s.remove_near_web_icons;
			this.draw_capture_info			= s.draw_capture_info;
			this.is_server_mode				= s.is_server_mode;
			this.port_index					= s.port_index;
			this.is_mixed_info_names		= s.is_mixed_info_names;
			this.minimum_draw_days			= s.minimum_draw_days;
			this.enable_favorite_sea_routes_alpha		= s.enable_favorite_sea_routes_alpha;
			this.draw_favorite_sea_routes_alpha_popup	= s.draw_favorite_sea_routes_alpha_popup;
			this.debug_flag_show_potision	= s.debug_flag_show_potision;
			this.enable_dpi_scaling		 = s.enable_dpi_scaling;

			// リクエストは전부취소
			CancelAllRequests();

			// 지도合成リクエストを업데이트する
			if(req){
				this.req_update_map.Request();
			}
		}

		/*-------------------------------------------------------------------------
		 설정파일읽기
		---------------------------------------------------------------------------*/
		public void IniLoad(IIni p, string group)
		{
		
			// 윈도우위치と사이즈
			m_window_location.X		= p.GetProfile("window", "pos_x",	window_location.X);
			m_window_location.Y		= p.GetProfile("window", "pos_y",	window_location.Y);
			m_window_size.Width		= p.GetProfile("window", "size_x",	window_size.Width);
			m_window_size.Height	= p.GetProfile("window", "size_y",	window_size.Height);

			m_find_window_location.X	= p.GetProfile("find_window", "pos_x",	find_window_location.X);
			m_find_window_location.Y	= p.GetProfile("find_window", "pos_y",	find_window_location.Y);
			m_find_window_size.Width	= p.GetProfile("find_window", "size_x",	find_window_size.Width);
			m_find_window_size.Height	= p.GetProfile("find_window", "size_y",	find_window_size.Height);
			find_window_visible		= p.GetProfile("find_window", "visible",	find_window_visible);

			m_sea_routes_window_location.X	= p.GetProfile("sea_routes_window", "pos_x",	sea_routes_window_location.X);
			m_sea_routes_window_location.Y	= p.GetProfile("sea_routes_window", "pos_y",	sea_routes_window_location.Y);
			m_sea_routes_window_size.Width	= p.GetProfile("sea_routes_window", "size_x",	sea_routes_window_size.Width);
			m_sea_routes_window_size.Height	= p.GetProfile("sea_routes_window", "size_y",	sea_routes_window_size.Height);
			sea_routes_window_visible		= p.GetProfile("sea_routes_window", "visible",	sea_routes_window_visible);

			save_searoutes			= p.GetProfile("icon", "save_searoutes",			save_searoutes);
			draw_share_routes		= p.GetProfile("icon", "draw_share_routes",			draw_share_routes);
			draw_icons				= p.GetProfile("icon", "draw_icons",				draw_icons);
			draw_sea_routes			= p.GetProfile("icon", "draw_sea_routes",			draw_sea_routes);
			draw_popup_day_interval	= p.GetProfile("icon", "draw_popup_day_interval",	draw_popup_day_interval);
			draw_accident			= p.GetProfile("icon", "draw_accident",				draw_accident);
			center_myship			= p.GetProfile("icon", "center_myship",				center_myship);
			draw_myship_angle		= p.GetProfile("icon", "draw_myship_angle",			draw_myship_angle);
			draw_web_icons			= p.GetProfile("icon", "draw_web_icons",			draw_web_icons);

			// ダイア로그설정항목
			server				  = GvoWorldInfo.GetServerFromString(p.GetProfile("dialog", "server", server.ToString()));
			country				 = GvoWorldInfo.GetCountryFromString(p.GetProfile("dialog", "country", country.ToString()));
			map						= MapIndex.Map1 + p.GetProfile("dialog", "map_index_new",	(int)map);
			map_icon				= MapIcon.Big + p.GetProfile("dialog", "map_icon",	(int)map_icon);
			map_draw_names			= MapDrawNames.Draw + p.GetProfile("dialog", "map_draw_names",	(int)map_draw_names);
			share_group				= p.GetProfile("dialog", "share_group",				share_group);
			share_group_myname		= p.GetProfile("dialog", "share_group_myname",		share_group_myname);
			searoutes_group_max		= p.GetProfile("dialog", "searoutes_group_max",		searoutes_group_max);
			trash_searoutes_group_max	= p.GetProfile("dialog", "trash_searoutes_group_max",		trash_searoutes_group_max);
			connect_network			= p.GetProfile("dialog", "connect_network",			connect_network);
			hook_mouse				= p.GetProfile("dialog", "hook_mouse",				hook_mouse);
			windows_vista_aero		= p.GetProfile("dialog", "windows_vista_aero",		windows_vista_aero);
			enable_analize_log_chat	= p.GetProfile("dialog", "enable_analize_log_chat",	enable_analize_log_chat);
			is_share_routes			= p.GetProfile("dialog", "is_share_routes",			is_share_routes);
			capture_interval		= CaptureIntervalIndex.Per500ms + p.GetProfile("dialog", "capture_interval",	(int)capture_interval);
			connect_web_icon		= p.GetProfile("dialog", "connect_web_icon",	connect_web_icon);
			compatible_windows_rclick	= p.GetProfile("dialog", "compatible_windows_rclick",	compatible_windows_rclick);
			this.tude_interval		= TudeInterval.None + p.GetProfile("dialog", "tude_interval",	(int)this.tude_interval);
			use_mixed_map			= p.GetProfile("dialog", "use_mixed_map",	use_mixed_map);
			window_top_most			= p.GetProfile("dialog", "window_top_most",	window_top_most);
			enable_line_antialias	= p.GetProfile("dialog", "enable_line_antialias",	enable_line_antialias);
			enable_sea_routes_aplha	= p.GetProfile("dialog", "enable_sea_routes_aplha",	enable_sea_routes_aplha);
			this.ss_format			= SSFormat.Bmp + p.GetProfile("dialog", "ss_format",	(int)this.ss_format);
			remove_near_web_icons	= p.GetProfile("dialog", "remove_near_web_icons",	remove_near_web_icons);
			draw_capture_info		= p.GetProfile("dialog", "draw_capture_info",	draw_capture_info);
			is_server_mode			= p.GetProfile("dialog", "is_server_mode",	is_server_mode);
			port_index				= p.GetProfile("dialog", "port_index",	port_index);
			is_mixed_info_names		= p.GetProfile("dialog", "is_mixed_info_names",	is_mixed_info_names);
			minimum_draw_days		= p.GetProfile("dialog", "minimum_draw_days",	minimum_draw_days);
			enable_favorite_sea_routes_alpha		= p.GetProfile("dialog", "enable_favorite_sea_routes_alpha",	enable_favorite_sea_routes_alpha);
			draw_favorite_sea_routes_alpha_popup	= p.GetProfile("dialog", "draw_favorite_sea_routes_alpha_popup",	draw_favorite_sea_routes_alpha_popup);
			debug_flag_show_potision	= p.GetProfile("dialog", "debug_flag_show_potision",	debug_flag_show_potision);
			enable_dpi_scaling		  = p.GetProfile("dialog", "enable_dpi_scaling", enable_dpi_scaling);

			// 표시関係
			m_select_info			= p.GetProfile("map", "select_info",		m_select_info);
			map_pos_x				= p.GetProfile("map", "map_pos_x",			map_pos_x);
			map_pos_y				= p.GetProfile("map", "map_pos_y",			map_pos_y);
			map_scale				= p.GetProfile("map", "map_scale",			map_scale);

			// 윈도우상태
			is_item_window_normal_size		= p.GetProfile("item_window", "is_normal_size",		is_item_window_normal_size);
			is_setting_window_normal_size	= p.GetProfile("setting_window", "is_normal_size",		is_setting_window_normal_size);

			is_border_style_none	= p.GetProfile("setting_window", "is_border_style_none",		is_border_style_none);

			// 이자からの경과일수
			this.interest_days		= p.GetProfile("interest", "interest_days",	this.interest_days);

			// 조선からの경과일수
			this.force_show_build_ship	= p.GetProfile("build_ship", "force_show_build_ship",	this.force_show_build_ship);
			this.is_now_build_ship	= p.GetProfile("build_ship", "is_now_build_ship",	this.is_now_build_ship);
			this.build_ship_name	= p.GetProfile("build_ship", "build_ship_name",	this.build_ship_name);
			this.build_ship_days	= p.GetProfile("build_ship", "build_ship_days",	this.build_ship_days);

			// 검색履歴
			int	index	= 0;
			m_find_strings.Clear();
			while(true){
				string		str		= p.GetProfile("find", String.Format("list{0}", index),	"");
				if(str == "")	break;
				m_find_strings.AddLast(str);
				index++;
			}
			find_filter				= _find_filter.world_info + p.GetProfile("find", "find_filter",		(int)find_filter);
			find_filter2			= _find_filter2.name + p.GetProfile("find", "find_filter2",		(int)find_filter2);
			find_filter3			= _find_filter3.full_text_search + p.GetProfile("find", "find_filter3",		(int)find_filter3);

			// 표시항목
			this.draw_setting_web_icons		= (DrawSettingWebIcons)p.GetProfile("draw_setting", "draw_setting_web_icons",	(int)this.draw_setting_web_icons);
			this.draw_setting_memo_icons	= (DrawSettingMemoIcons)p.GetProfile("draw_setting", "draw_setting_memo_icons",	(int)this.draw_setting_memo_icons);
			this.draw_setting_accidents		= (DrawSettingAccidents)p.GetProfile("draw_setting", "draw_setting_accidents",	(int)this.draw_setting_accidents);
			this.draw_setting_myship_angle	= (DrawSettingMyShipAngle)p.GetProfile("draw_setting", "draw_setting_myship_angle",	(int)this.draw_setting_myship_angle);
			this.draw_setting_myship_angle_with_speed_pos	= p.GetProfile("draw_setting", "draw_setting_myship_angle_with_speed_pos",	this.draw_setting_myship_angle_with_speed_pos);
			this.draw_setting_myship_expect_pos	= p.GetProfile("draw_setting", "draw_setting_myship_expect_pos",	this.draw_setting_myship_expect_pos);
		}

		/*-------------------------------------------------------------------------
		 설정파일書き出し
		---------------------------------------------------------------------------*/
		public void IniSave(IIni p, string group)
		{
			// 윈도우위치と사이즈
			p.SetProfile("window", "pos_x",					window_location.X);
			p.SetProfile("window", "pos_y",					window_location.Y);
			p.SetProfile("window", "size_x",					window_size.Width);
			p.SetProfile("window", "size_y",					window_size.Height);

			p.SetProfile("find_window", "pos_x",				find_window_location.X);
			p.SetProfile("find_window", "pos_y",				find_window_location.Y);
			p.SetProfile("find_window", "size_x",				find_window_size.Width);
			p.SetProfile("find_window", "size_y",				find_window_size.Height);
			p.SetProfile("find_window", "visible",			find_window_visible);

			p.SetProfile("sea_routes_window", "pos_x",		sea_routes_window_location.X);
			p.SetProfile("sea_routes_window", "pos_y",		sea_routes_window_location.Y);
			p.SetProfile("sea_routes_window", "size_x",		sea_routes_window_size.Width);
			p.SetProfile("sea_routes_window", "size_y",		sea_routes_window_size.Height);
			p.SetProfile("sea_routes_window", "visible",		sea_routes_window_visible);

			p.SetProfile("icon", "save_searoutes",			save_searoutes);
			p.SetProfile("icon", "draw_share_routes",			draw_share_routes);
			p.SetProfile("icon", "draw_icons",				draw_icons);
			p.SetProfile("icon", "draw_sea_routes",			draw_sea_routes);
			p.SetProfile("icon", "draw_popup_day_interval",	draw_popup_day_interval);
			p.SetProfile("icon", "draw_accident",				draw_accident);
			p.SetProfile("icon", "center_myship",				center_myship);
			p.SetProfile("icon", "draw_myship_angle",			draw_myship_angle);
			p.SetProfile("icon", "draw_web_icons",			draw_web_icons);

			// ダイア로그설정항목
			p.SetProfile("dialog", "server",					server.ToString());
			p.SetProfile("dialog", "country",					country.ToString());
			p.SetProfile("dialog", "map_index_new",			(int)map);
			p.SetProfile("dialog", "map_icon",				(int)map_icon);
			p.SetProfile("dialog", "map_draw_names",			(int)map_draw_names);
			p.SetProfile("dialog", "share_group",				share_group);
			p.SetProfile("dialog", "share_group_myname",		share_group_myname);
			p.SetProfile("dialog", "searoutes_group_max",		searoutes_group_max);
			p.SetProfile("dialog", "trash_searoutes_group_max",	trash_searoutes_group_max);
			p.SetProfile("dialog", "connect_network",			connect_network);
			p.SetProfile("dialog", "hook_mouse",				hook_mouse);
			p.SetProfile("dialog", "windows_vista_aero",		windows_vista_aero);
			p.SetProfile("dialog", "enable_analize_log_chat",	enable_analize_log_chat);
			p.SetProfile("dialog", "is_share_routes",			is_share_routes);
			p.SetProfile("dialog", "capture_interval",		(int)capture_interval);
			p.SetProfile("dialog", "connect_web_icon",		connect_web_icon);
			p.SetProfile("dialog", "compatible_windows_rclick",	compatible_windows_rclick);
			p.SetProfile("dialog", "tude_interval",			(int)this.tude_interval);
			p.SetProfile("dialog", "use_mixed_map",			use_mixed_map);
			p.SetProfile("dialog", "window_top_most",			window_top_most);
			p.SetProfile("dialog", "enable_line_antialias",	enable_line_antialias);
			p.SetProfile("dialog", "enable_sea_routes_aplha",	enable_sea_routes_aplha);
			p.SetProfile("dialog", "ss_format",				(int)this.ss_format);
			p.SetProfile("dialog", "remove_near_web_icons",	remove_near_web_icons);
			p.SetProfile("dialog", "draw_capture_info",		draw_capture_info);
			p.SetProfile("dialog", "is_server_mode",			is_server_mode);
			p.SetProfile("dialog", "port_index",				port_index);
			p.SetProfile("dialog", "is_mixed_info_names",		is_mixed_info_names);
			p.SetProfile("dialog", "minimum_draw_days",		minimum_draw_days);
			p.SetProfile("dialog", "enable_favorite_sea_routes_alpha",	enable_favorite_sea_routes_alpha);
			p.SetProfile("dialog", "draw_favorite_sea_routes_alpha_popup",	draw_favorite_sea_routes_alpha_popup);
			p.SetProfile("dialog", "debug_flag_show_potision",	debug_flag_show_potision);
			p.SetProfile("dialog", "enable_dpi_scaling", enable_dpi_scaling);

			// 표시関係
			p.SetProfile("map", "select_info",				m_select_info);
			p.SetProfile("map", "map_pos_x",					map_pos_x);
			p.SetProfile("map", "map_pos_y",					map_pos_y);
			p.SetProfile("map", "map_scale",					map_scale);

			// 윈도우상태
			p.SetProfile("item_window", "is_normal_size",		is_item_window_normal_size);
			p.SetProfile("setting_window", "is_normal_size",		is_setting_window_normal_size);

			p.SetProfile("setting_window", "is_border_style_none",		is_border_style_none);

			// 이자からの경과일수
			p.SetProfile("interest", "interest_days",	this.interest_days);

			// 조선からの경과일수
			p.SetProfile("build_ship", "force_show_build_ship",	this.force_show_build_ship);
			p.SetProfile("build_ship", "is_now_build_ship",	this.is_now_build_ship);
			p.SetProfile("build_ship", "build_ship_name",	this.build_ship_name);
			p.SetProfile("build_ship", "build_ship_days",	this.build_ship_days);

			// 검색履歴
			int	index	= 0;
			foreach(string s in m_find_strings){
				p.SetProfile("find", String.Format("list{0}", index),	s);
				index++;
			}
			p.SetProfile("find", "find_filter",		(int)find_filter);
			p.SetProfile("find", "find_filter2",		(int)find_filter2);
			p.SetProfile("find", "find_filter3",		(int)find_filter3);

			// 표시항목
			p.SetProfile("draw_setting", "draw_setting_web_icons",	(int)this.draw_setting_web_icons);
			p.SetProfile("draw_setting", "draw_setting_memo_icons",	(int)this.draw_setting_memo_icons);
			p.SetProfile("draw_setting", "draw_setting_accidents",	(int)this.draw_setting_accidents);
			p.SetProfile("draw_setting", "draw_setting_myship_angle",	(int)this.draw_setting_myship_angle);
			p.SetProfile("draw_setting", "draw_setting_myship_angle_with_speed_pos",	this.draw_setting_myship_angle_with_speed_pos);
			p.SetProfile("draw_setting", "draw_setting_myship_expect_pos",	this.draw_setting_myship_expect_pos);
		}
	}
}
