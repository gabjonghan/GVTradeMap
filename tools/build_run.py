import glob, os, re, shutil, subprocess, sys
# VS/SDK 없이 리소스까지 묶어 실행 가능한 앱을 만든다. ILMerge 는 하지 않으므로 exe + DLL 구성이다.
# usage: python tools/build_run.py <출력 폴더>   (출력 폴더의 gvtrademap_cs_.exe 를 그 폴더를 작업 폴더로 실행)
R = os.path.dirname(os.path.dirname(os.path.abspath(__file__))).replace("\\", "/")
O = os.path.abspath(sys.argv[1]).replace("\\", "/")
OBJ = O + "/obj"
os.makedirs(OBJ, exist_ok=True)
FW = "C:/Windows/Microsoft.NET/Framework/v2.0.50727/"
MDX = "C:/Windows/Microsoft.NET/DirectX for Managed Code/"
CSC = glob.glob("C:/Program Files/dotnet/sdk/*/Roslyn/bincore/csc.dll")[0]
BASE = ["-nologo", "-nostdlib+", "-noconfig", "-langversion:6", "-platform:x86"]


def run(args):
    r = subprocess.run(["dotnet", "exec", CSC] + args, capture_output=True)
    out = r.stdout.decode("utf-8", "replace")
    errs = [l for l in out.splitlines() if ": error" in l]
    return r.returncode, errs


# resx -> resources 변환기
tool = OBJ + "/resx.cs"
open(tool, "w", encoding="utf-8").write(r'''
using System;using System.Collections;using System.Resources;using System.IO;
class T{static int Main(string[] a){
 Directory.SetCurrentDirectory(Path.GetDirectoryName(a[0]));
 using(ResXResourceReader r=new ResXResourceReader(a[0])){ using(ResourceWriter w=new ResourceWriter(a[1])){
  foreach(DictionaryEntry e in r) w.AddResource((string)e.Key,e.Value); w.Generate(); }}
 return 0;}}
''')
rc, e = run(BASE + ["-target:exe", "-out:" + OBJ + "/resx.exe"] + ["-r:" + FW + x + ".dll" for x in ("mscorlib", "System", "System.Windows.Forms", "System.Drawing")] + [tool])
assert rc == 0, e
open(OBJ + "/resx.exe.config", "w").write('<configuration><startup><supportedRuntime version="v2.0.50727"/></startup></configuration>')


def class_of(cs):
    t = open(cs, encoding="utf-8-sig", errors="replace").read()
    ns = re.search(r"namespace\s+([\w.]+)", t).group(1)
    c = re.search(r"class\s+(\w+)", t).group(1)
    return ns + "." + c


P = ["library_cs/utility/utility", "library_cs/PanelManager/PanelManager", "library_cs/net/net", "library_cs/gvo_base/gvo_base", "library_cs/directx/directx", "library_cs/gvo_net_base/gvo_net_base", "gvtrademap_cs/gvtrademap_cs"]
built = []
for p in P:
    cs = R + "/" + p + ".csproj"
    d = os.path.dirname(cs)
    t = open(cs, encoding="utf-8-sig").read()
    root = re.search(r"<RootNamespace>(.*?)</RootNamespace>", t).group(1)
    asm = re.search(r"<AssemblyName>(.*?)</AssemblyName>", t).group(1)
    src = [os.path.join(d, s.replace("\\", "/")) for s in re.findall(r'<Compile Include="([^"]+)"', t)]
    refs = ["mscorlib"] + [r.split(",")[0] for r in re.findall(r'<Reference Include="([^"]+)"', t)]
    a = BASE + ["-define:DEBUG;TRACE", "-unsafe+", "-debug+", "-nowarn:CS1701,CS1702,CS0618,CS0612,CS0219,CS0168,CS0162,CS0169,CS0414,CS0649,CS0108,CS0114,CS1591,CS0105"]
    is_exe = p.endswith("gvtrademap_cs")
    a += ["-target:" + ("winexe" if is_exe else "library"), "-out:%s/%s.%s" % (O, asm, "exe" if is_exe else "dll")]
    for r in refs:
        if r.startswith("Microsoft.DirectX"):
            a.append("-r:%s%s/%s.dll" % (MDX, "1.0.2911.0" if r.endswith("3DX") else "1.0.2902.0", r))
        else:
            a.append("-r:%s%s.dll" % (FW, r))
    a += ["-r:" + b for b in built]
    for m in re.finditer(r'<EmbeddedResource Include="([^"]+)"\s*(/>|>(.*?)</EmbeddedResource>)', t, re.S):
        inc, body = m.group(1), m.group(3) or ""
        path = os.path.join(d, inc.replace("\\", "/")).replace("\\", "/")
        if inc.endswith(".resx"):
            dep = re.search(r"<DependentUpon>(.*?)</DependentUpon>", body)
            cs_file = path[:-5] + ".cs"
            if os.path.exists(cs_file) and not path.endswith("Resources.resx"):
                name = class_of(cs_file) + ".resources"
            else:
                name = root + "." + inc[:-5].replace("\\", ".") + ".resources"
            res = OBJ + "/" + name
            r = subprocess.run([OBJ + "/resx.exe", os.path.abspath(path), os.path.abspath(res)], capture_output=True)
            assert r.returncode == 0, (path, r.stderr[:400])
            a.append("-resource:%s,%s" % (res, name))
        else:
            a.append("-resource:%s,%s.%s" % (path, root, inc.replace("\\", ".")))
    a += src
    rsp = OBJ + "/" + asm + ".rsp"
    open(rsp, "w", encoding="utf-8").write("\n".join('"%s"' % x if " " in x else x for x in a))
    rc, errs = run(["@" + rsp])
    print(asm, "exit", rc, "errors", len(errs))
    for l in errs[:8]:
        print("  ", l[:200].encode("ascii", "backslashreplace").decode())
    if rc:
        sys.exit(1)
    if not is_exe:
        built.append("%s/%s.dll" % (O, asm))

# 실행에 필요한 폴더
G = R + "/gvtrademap_cs"
for sub in ("database", "map", "font", "image"):
    dst = O + "/" + sub
    if os.path.isdir(G + "/" + sub):
        shutil.copytree(G + "/" + sub, dst, dirs_exist_ok=True)
for sub in ("temp", "memo", "SS"):
    os.makedirs(O + "/" + sub, exist_ok=True)
shutil.copy(G + "/app.config", O + "/gvtrademap_cs_.exe.config")
print("ok")
