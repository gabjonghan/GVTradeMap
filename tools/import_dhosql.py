# dhoSQL(오프라인 조이, 2025-10 기준 SQLite)을 앱 데이터 파일로 바꾼다.
#   usage: python tools/import_dhosql.py <dhoDatabase.sqlite3> [--write]
#   DB: https://github.com/dhosql/dhoSQL (sql/dhoDatabase.sqlite3). DB 파일은 저장소에 넣지 않는다.
#
# 만드는 것
#   database/cityinfos.xml  도시별 판매 품목([교역]~[행상인])과 [할 일](투자·거래 보상, 낚시, 서고 지도)
#                           [인물]·[조선공]·[부관] 과 판매 자료가 없는 장소의 그룹은 지금 파일 값을 그대로 둔다
#   database/gameinfo/      01_침몰선, 02_낚시, 03_기상현상, 12_서고 지도
# --write 없이 돌리면 통계만 출력한다.
import io, os, re, sqlite3, sys
from xml.sax.saxutils import quoteattr

DB = sys.argv[1]
WRITE = "--write" in sys.argv
ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
D = os.path.join(ROOT, "gvtrademap_cs", "database")
GI = os.path.join(D, "gameinfo")
SRC = "dhoSQL(오프라인 조이) 2025-10 https://github.com/dhosql/dhoSQL"

c = sqlite3.connect("file:%s?mode=ro" % DB.replace("\\", "/"), uri=True)
q = lambda s, a=(): list(c.execute(s, a))

# dhoSQL 표기 -> 앱 장소명
ALIAS = {}

wt = io.open(os.path.join(D, "worldinfos.xml"), encoding="utf-8-sig").read()
world = {}
for m in re.finditer(r'<info name="([^"]+)" [^>]*info_type="(\w+)"[^>]*>\s*<position x="(-?\d+)" y="(-?\d+)"', wt):
    world[m.group(1)] = (m.group(2), int(m.group(3)), int(m.group(4)))


def place(name):
    name = ALIAS.get(name.strip(), name.strip())
    return name, world.get(name)


def num(s):
    s = re.sub(r"[^\d-]", "", str(s or ""))
    return int(s) if s not in ("", "-") else None


def kfmt(v):
    v = num(v)
    if v is None:
        return ""
    return "%dk" % (v // 1000) if v % 1000 == 0 else str(v)


# allData: id -> (분류, 이름)
ad = {}
for i, n, cat in q("select id,name,category from allData"):
    ad[str(i)] = (cat, re.sub(r"^.*?\] ", "", n))
ids = lambda s: re.findall(r"\d+", s or "")


def pos_of_id(i):
    cat, name = ad.get(str(i), ("", ""))
    return name, place(name)[1]


report = []

# ------------------------------------------------------------ 도시 판매
NPC_GROUP = {
    "교역소주인": "[교역]", "도구점 주인": "[도구]", "대장장이": "[공방]", "대장장이 도제": "[공방]",
    "조선소주인": "[배]", "무기장인": "[대포]", "무기 장인": "[대포]", "제재소장인": "[추가장갑]",
    "돛제작자": "[돛]", "돛 제작자": "[돛]", "조각가": "[선수상]",
}
ORDER = ["[교역]", "[도구]", "[공방]", "[인물]", "[배]", "[조선공]", "[대포]", "[추가장갑]", "[돛]", "[선수상]", "[행상인]", "[부관]", "[할 일]"]
KEEP = {"[인물]", "[조선공]", "[부관]"}       # 지금 파일 값을 유지하는 그룹

ITEM_DB = os.path.join(D, "item_db.txt")
ADD_ID = 9100001         # 이 스크립트가 item_db 에 넣는 행의 시작 ID. 다시 돌리면 이 범위를 지우고 새로 넣는다
db_lines = [l for l in io.open(ITEM_DB, encoding="utf-8-sig").read().split("\n") if l and not re.match(r"91\d{5},", l)]
items = set(l.split(",")[2] for l in db_lines if l.count(",") >= 2)
nitems = {re.sub(r"[\s()]", "", i): i for i in items}
SUFFIX = re.compile(r"\((구입|금괴교환|교역품|식료품|머리|[^)]*구입[^)]*|대부호|시대 한정[^)]*)\)$")


def item_name(n):
    """dhoSQL 품목명 -> (item_db 이름, 입수 방법 메모)"""
    # item_db 는 쉼표로 열을 나누므로 이름 속 쉼표는 가운뎃점으로 바꾼다(예: 스웨덴 민족 의상(구입, ♂))
    n = re.sub(r"\s*,\s*", "·", n.strip())
    if n in items:
        return n, ""
    m = SUFFIX.search(n)
    if m:
        b = n[:m.start()].strip()
        if b in items:
            return b, m.group(1)
        if re.sub(r"\s", "", b) in nitems:
            return nitems[re.sub(r"\s", "", b)], m.group(1)
    k = re.sub(r"[\s()]", "", n)
    return (nitems[k], "") if k in nitems else (n, "")


sales = {}
skipped_city = set()
for npc, city, cat, item, price, invest in q("select npc,city,item_category,item,price,invest from npcsale"):
    # 조합 판매처는 그 도시의 행상인 칸에 넣는다
    m = re.match(r"(.+?) (모험가조합|상인조합|해양조합|종합 조합)$", city)
    if m:
        city, npc = m.group(1), (npc if npc.startswith(m.group(2)[:2]) else m.group(2) + " " + npc)
    name, p = place(city)
    if not p:
        skipped_city.add(city)
        continue
    g = NPC_GROUP.get(npc, "[행상인]")
    iname, note = item_name(item)
    attrs = [("name", iname)]
    if invest:
        attrs.append(("option", "*"))
    pr = num(price)
    # dhoSQL 의 교역소 가격은 대부분 세금(def.TAX 14.3%)이 붙은 값이다(이전 데이터 대비 중앙값 1.14배).
    # 앱은 기본가를 저장하고 동맹국이 아닐 때 세금을 더하므로 기본가로 되돌린다
    if g == "[교역]" and pr:
        pr = int(round(pr / 1.143))
    attrs.append(("price", str(pr) if pr is not None else "-1"))
    if invest:
        attrs.append(("investment", kfmt(invest)))
    if cat:
        attrs.append(("kind", cat))
    if g == "[행상인]" or npc not in NPC_GROUP:
        attrs.append(("npc", npc))
    if note:
        attrs.append(("note", note))
    lst = sales.setdefault(name, {}).setdefault(g, [])
    if attrs not in lst:
        lst.append(attrs)

# 할 일: 투자·거래 보상, 낚시
todo = {}
for name0, inv, invr, deal, dealr, fish in q('select name,"투자 금액","투자 보상","거래 금액","거래 보상","낚시" from city'):
    name, p = place(name0)
    if not p:
        continue
    t = todo.setdefault(name, [])
    if inv and invr:
        t.append([("name", "투자 " + inv), ("option", "~"), ("price", invr), ("kind", "투자")])
    if deal and dealr:
        t.append([("name", "거래 " + deal), ("option", "~"), ("price", dealr), ("kind", "거래")])
    if fish:
        t.append([("name", "낚시"), ("option", "~"), ("price", re.sub(r"\s+", " ", fish)), ("kind", "낚시")])
# 할 일: 서고 지도(서고 도시별)
tmap_rows = q("select name,required_skill,category,library,destination,discovery from treasuremap")
for name0, skill, cat, lib, dest, disc in tmap_rows:
    if cat != "서고" or not lib:
        continue
    dname = ad.get(str(dest), ("", ""))[1]
    for city in [x.strip() for x in lib.split(",")]:
        name, p = place(city)
        if p:
            todo.setdefault(name, []).append([("name", name0), ("option", "~"), ("price", "%s → %s" % (skill or "", dname)), ("kind", "서고")])

# 지금 파일에서 유지할 그룹과 장소 순서
#   [인물]·[조선공]·[부관] 과, dhoSQL 에 판매 자료가 없는 장소(해역·상륙지의 채집 품목 등)의 그룹은 지금 파일 값을 그대로 둔다
def parse_blocks(text):
    res = []
    for name, body in re.findall(r'  <cityinfo name="([^"]+)">\n(.*?)  </cityinfo>\n', text, re.S):
        res.append((name, re.findall(r'\t<group name="([^"]+)">\n(.*?)\t</group>\n', body, re.S)))
    return res


# 아이템 이름을 바꿨을 때의 대응표(tools/item_renames.tsv: 옛 이름<TAB>새 이름). 유지하는 그룹에 적용한다
ITEM_RENAME = {}
_rf = os.path.join(ROOT, "tools", "item_renames.tsv")
if os.path.exists(_rf):
    for l in io.open(_rf, encoding="utf-8"):
        a = l.rstrip("\n").split("\t")
        if len(a) == 2:
            ITEM_RENAME[a[0]] = a[1]


def rename_items(body):
    return re.sub(r'(<item name=")([^"]+)(")', lambda m: m.group(1) + ITEM_RENAME.get(m.group(2), m.group(2)) + m.group(3), body)


cur_blocks = parse_blocks(io.open(os.path.join(D, "cityinfos.xml"), encoding="utf-8-sig").read())
cur_order = [b[0] for b in cur_blocks]
kept = {}
for name, gs in cur_blocks:
    for gname, gbody in gs:
        if gname in KEEP or (name not in sales and gname in ORDER and gname != "[할 일]"):
            kept.setdefault(name, {})[gname] = rename_items(gbody)

names = [n for n in cur_order if n in sales or n in kept or n in todo]
names += sorted(n for n in set(sales) | set(todo) if n not in names)
dropped = [n for n in cur_order if n not in names]

out = ['﻿<?xml version="1.0" encoding="UTF-8"?>', "<cityinfo_root>",
       "  <!-- 도시정보. 판매 품목·할 일: %s. [인물]·[조선공]·[부관]: 인벤 지리 DB·원본 -->" % SRC, "  "]
n_items = 0
for name in names:
    out.append('  <cityinfo name=%s>' % quoteattr(name))
    for g in ORDER:
        if g in KEEP or (name not in sales and g != "[할 일]"):
            body = kept.get(name, {}).get(g)
            if body:
                out.append('\t<group name="%s">' % g)
                out.append(body.rstrip("\n"))
                out.append("\t</group>")
            continue
        rows = todo.get(name) if g == "[할 일]" else sales.get(name, {}).get(g)
        if not rows:
            continue
        out.append('\t<group name="%s">' % g)
        for a in rows:
            out.append("\t  <item %s />" % " ".join("%s=%s" % (k, quoteattr(v)) for k, v in a))
            n_items += 1
        out.append("\t</group>")
    out.append("  </cityinfo>")
out.append("  </cityinfo_root>")

miss = {}
for name in sales:
    for g, rows in sales[name].items():
        for a in rows:
            if a[0][1] not in items:
                miss[a[0][1]] = dict(a).get("kind", "")
# item_db 에 없는 판매 품목은 분류를 맞춰 행을 추가한다. 선박은 크기 분류를 알 수 없어 넣지 않는다
KIND_TYPE = [(r"장비품 \(몸\)", "몸 장비품"), (r"장비품 \(머리\)", "머리 장비품"), (r"장비품 \(다리\)", "다리 장비품"),
             (r"장비품 \(팔\)", "팔 장비품"), (r"장비품 \(장신구\)", "장신구"), (r"장비품", "무기/도구"),
             (r"대포 \(선측포\)", "선측포"), (r"대포 \(선미포\)", "선미포"), (r"대포 \(선수포\)", "선수포"),
             (r"레시피 책", "레시피"), (r"추가장갑", "추가장갑"), (r"아이템 효과", "아이템 효과"), (r"소비품|선박 데코", "소비품"),
             (r"교역품 \((\S+)\)", None)]
add_rows = []
for n, kind in sorted(miss.items()):
    t = None
    for pat, typ in KIND_TYPE:
        m = re.match(pat, kind)
        if m:
            t = typ or m.group(1)
            break
    if t:
        add_rows.append("%d,%s,%s,dhoSQL(오프라인 조이) 판매 목록 기준으로 추가한 아이템." % (ADD_ID + len(add_rows), t, n))
report.append("item_db 추가 %d행" % len(add_rows))
report.append("장소 %d곳(판매 %d, 할 일 %d), 행 %d. 지금 파일에서 빠지는 장소 %d: %s" % (len(names), len(sales), len(todo), n_items, len(dropped), dropped))
report.append("장소를 못 찾아 건너뛴 판매처 %d: %s" % (len(skipped_city), sorted(skipped_city)))
report.append("item_db 에 없는 판매 품목 %d종" % len(miss))

# ------------------------------------------------------------ 게임 자료
def gi(fname, head, groups):
    lines = [head, "// 출처: " + SRC, ""]
    np = 0
    for g, pts in groups:
        if not pts:
            continue
        lines.append("// " + g)
        for label, x, y in pts:
            if x is None:
                lines.append(label)
            else:
                lines.append("%s=%d,%d" % (label.replace("=", "-"), x, y))
                np += 1
        lines.append("")
    if WRITE:
        io.open(os.path.join(GI, fname), "w", encoding="utf-8-sig", newline="\n").write("\n".join(lines) + "\n")
    return np


def xy(s):
    m = re.search(r"(\d{2,5})\D+(\d{2,5})", s or "")
    return (int(m.group(1)), int(m.group(2))) if m else None


# 침몰선: 좌표가 있으면 그 좌표, 없으면 목적지 해역 위치(대략)
grp = {}
exact = approx = 0
for name0, expl, diff, sea, dest, coord, skill, trade, cons, equip in q(
        "select name,explanation,difficulty,sea_area,destination,discovery_coordinates,skill,trade_goods,consumables,equipment from shipwreck order by sea_area,destination,name"):
    ship = re.search(r"선박종류\s*-\s*([^\r\n]+)", expl or "")
    loot = [y.strip() for x in [trade, cons] if x for y in x.split(",")]
    loot = ", ".join(loot[:6]) + (" 외 %d" % (len(loot) - 6) if len(loot) > 6 else "")
    label = "%s ★%s%s%s%s" % (name0, diff or "?", " " + skill if skill else "", " · " + ship.group(1).strip() if ship else "", " · " + loot if loot else "")
    p = xy(coord)
    if p:
        exact += 1
    else:
        dp = place(dest or "")[1]
        if dp:
            p = (dp[1], dp[2])
            label += " (해역 위치, 대략)"
            approx += 1
    grp.setdefault(sea or "기타", []).append((label + ("" if p else " — " + (dest or "위치 미상")), p[0] if p else None, p[1] if p else None))
gi("01_침몰선.txt", "침몰선 위치. 좌표가 정해진 곳은 그 좌표, 아니면 목적지 해역 위치(대략)에 찍습니다. ★ 난이도", sorted(grp.items()))
report.append("침몰선 %d (정확 %d, 해역 위치 %d)" % (sum(len(v) for v in grp.values()), exact, approx))


def disc_groups(where):
    grp, ex, ap = {}, 0, 0
    for name0, cat, method, rank, loc, coord, era, tp, wthr, sea in q(
            "select name,category,discovery_method,discovery_rank,discovery_location,coordinates,era,time_period,weather,'' from discovery where " + where + " order by name"):
        cond = " ".join(x for x in [rank, tp, wthr, era] if x)
        label = name0 + (" (" + cond + ")" if cond else "")
        pts = [(int(a), int(b)) for a, b in re.findall(r"\[(\d+),\s*(\d+)\]", coord or "")]
        locs = [pos_of_id(i) for i in ids(loc)]
        gname = locs[0][0] if locs else "기타"
        if pts:
            ex += 1
            for x, y in pts:
                grp.setdefault(gname, []).append((label, x, y))
            continue
        hit = [(n, p) for n, p in locs if p]
        if hit:
            ap += 1
            for n, p in hit:
                grp.setdefault(n, []).append((label + " (장소 위치, 대략)", p[1], p[2]))
        else:
            grp.setdefault(gname, []).append((label + " — " + ", ".join(n for n, _ in locs), None, None))
    return sorted(grp.items()), ex, ap


g2, e2, a2 = disc_groups("discovery_method like '%낚시%'")
gi("02_낚시.txt", "낚시로 얻는 발견물. 좌표가 없는 것은 발견 장소(해역·상륙지) 위치(대략)에 찍습니다.", g2)
report.append("낚시 정확 %d, 대략 %d" % (e2, a2))
g3, e3, a3 = disc_groups("category='기상 현상'")
gi("03_기상현상.txt", "기상 현상. 괄호 안은 인식 랭크, 시간대, 날씨입니다. 좌표가 없는 것은 발견 장소 위치(대략)에 찍습니다.", g3)
report.append("기상 정확 %d, 대략 %d" % (e3, a3))

# 서고 지도 등 보물지도: 목적지 기준
grp = {}
nmap = 0
for name0, skill, cat, lib, dest, disc in tmap_rows:
    dname, p = pos_of_id(dest)
    dn = ad.get(str(disc), ("", ""))[1]
    label = "%s [%s] %s%s%s" % (name0, cat or "", skill or "", " → " + dn if dn else "", " 서고: " + lib if lib else "")
    grp.setdefault(dname or "기타", []).append((label, p[1] if p else None, p[2] if p else None))
    nmap += 1
gi("12_보물지도.txt", "보물지도의 목적지. 지도의 점은 목적지(해역·상륙지·도시) 위치(대략)입니다. [ ] 는 얻는 곳(서고, NPC, 육지 재조사 등).", sorted(grp.items()))
report.append("보물지도 %d" % nmap)

print("\n".join(report))
if WRITE:
    io.open(os.path.join(D, "cityinfos.xml"), "w", encoding="utf-8", newline="\n").write("\n".join(out) + "\n")
    io.open(ITEM_DB, "w", encoding="utf-8-sig", newline="\r\n").write("\n".join(db_lines + add_rows) + "\n")
