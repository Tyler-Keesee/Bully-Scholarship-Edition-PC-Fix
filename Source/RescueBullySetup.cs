using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Runtime.InteropServices;

class FileChange { public string Target, Backup, After; public bool Existed; }
class InstallRecord { public string Game; public List<FileChange> Files=new List<FileChange>(); public Dictionary<string,int?> Registry=new Dictionary<string,int?>(); public Dictionary<string,int> Applied=new Dictionary<string,int>(); }
class RescueSetup : Form {
 const string ExeHash="36FF31682081A61BA564CB5E09CD6240F79791879CF1FBFB8E6252F75FADDC6C";
 const string RegPath="Software\\Rockstar Games\\Bully Scholarship Edition\\A0";
 readonly string root=AppDomain.CurrentDomain.BaseDirectory;
 readonly JavaScriptSerializer json=new JavaScriptSerializer();
 TextBox folder; NumericUpDown width,height,fov; ComboBox fps; CheckBox applyControls,render; DataGridView bindings; Label status;
 static string[] Actions={"Menu left","Menu right","Menu up","Menu down","Map","Pause","Melee / punch","Sprint / social positive","Jump","Grapple / interact","Lock on / social negative","Previous weapon","Fire / throw","Next weapon","Look back","Crouch","Move left","Move forward","Camera left","Camera up","Move right","Move backward","Camera right","Camera down","Skateboard"};
 static int[] DefaultKeys={203,205,200,208,50,1,256,88,57,18,257,260,16,261,19,42,30,17,264,262,32,31,265,263,258};
 static string Hash(byte[] b){using(var h=SHA256.Create())return BitConverter.ToString(h.ComputeHash(b)).Replace("-","");}
 static string HashFile(string p){return Hash(File.ReadAllBytes(p));}
 [DllImport("user32.dll")] static extern uint MapVirtualKey(uint code,uint type);
 [DllImport("user32.dll")] static extern bool SetProcessDpiAwarenessContext(IntPtr value);
 [STAThread] static int Main(string[] args){
  if(args.Contains("--verify")){try{var app=new RescueSetup();app.Verify();app.SelfTest();File.WriteAllText(Path.Combine(app.root,"Verification.txt"),"PASS: payload SHA-256, prompt delta, control profile round-trip, combat binding repair, renderer removal and file rollback checks.\r\n"+DateTime.Now.ToString("o"));return 0;}catch(Exception ex){File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"Verification.txt"),"FAIL: "+ex.Message);return 1;}}
  SetProcessDpiAwarenessContext(new IntPtr(-4));Application.EnableVisualStyles();Application.SetCompatibleTextRenderingDefault(false);Application.Run(new RescueSetup());return 0;
 }
 public RescueSetup(){
  Text="RescuePC Bully Setup — Preview 0.1.1";ClientSize=new Size(870,700);MinimumSize=new Size(886,739);Font=new Font("Segoe UI",10);StartPosition=FormStartPosition.CenterScreen;BackColor=Color.FromArgb(245,247,250);Icon=Icon.ExtractAssociatedIcon(Application.ExecutablePath);
  AddLabel("Bully PC • RescuePC",22,16,20);AddLabel("rescuepcrepairs.com  |  Preview 0.1.1 — known issues remain",24,54,10);
  folder=new TextBox{Left=24,Top=86,Width=710,Text=FindGame()};Controls.Add(folder);ButtonAt("Browse",746,84,96,()=>{using(var d=new FolderBrowserDialog()){if(d.ShowDialog()==DialogResult.OK)folder.Text=d.SelectedPath;}});
  var monitor=Screen.PrimaryScreen.Bounds;AddLabel("Resolution",24,128,10);width=Number(112,126,640,16384,monitor.Width);height=Number(220,126,480,8640,monitor.Height);
  AddLabel("FOV factor",340,128,10);fov=Number(425,126,60,130,100);AddLabel("%",518,128,10);
  AddLabel("FPS",555,128,10);fps=new ComboBox{Left=592,Top=126,Width=80,DropDownStyle=ComboBoxStyle.DropDownList};fps.Items.AddRange(new object[]{"60","30"});fps.SelectedIndex=0;Controls.Add(fps);
  ButtonAt("Use display",688,123,154,()=>{var m=Screen.FromControl(this).Bounds;width.Value=m.Width;height.Value=m.Height;});
  AddLabel("Windowed borderless requested • VSync on • 4× AA requested • display settings apply next launch",24,166,9);
  render=new CheckBox{Text="Include experimental render module (filtering / presentation; flashing unresolved)",Left=24,Top=194,Width=820,Checked=false};Controls.Add(render);
  applyControls=new CheckBox{Text="Apply full control layout (shared melee / fire keys are always repaired; saves are never replaced)",Left=24,Top=224,Width=820,Checked=false};Controls.Add(applyControls);
  bindings=new DataGridView{Left=24,Top=258,Width=818,Height=290,AllowUserToAddRows=false,AllowUserToDeleteRows=false,RowHeadersVisible=false,AutoSizeColumnsMode=DataGridViewAutoSizeColumnsMode.Fill,BackgroundColor=Color.White};
  bindings.Columns.Add("Action","Action");bindings.Columns[0].ReadOnly=true;bindings.Columns.Add("Key","Binding — click then press a key, or type code");bindings.Columns.Add("Code","Native code");bindings.Columns[2].ReadOnly=true;
  for(int i=0;i<25;i++)bindings.Rows.Add(Actions[i],KeyName(DefaultKeys[i],i),DefaultKeys[i]);
  bindings.CellBeginEdit+=(s,e)=>{if(e.ColumnIndex==1&&e.RowIndex==7){e.Cancel=true;status.Text="Sprint uses Mouse 5 in this preview. The custom M5 prompt matches this binding.";}};
  bindings.EditingControlShowing+=(s,e)=>{e.Control.KeyDown-=CaptureKey;e.Control.KeyDown+=CaptureKey;};Controls.Add(bindings);
  AddLabel("Mouse codes: 256 left • 257 right • 258 middle • 259 side 4 • 260/261 wheel up/down",24,557,9);
  ButtonAt("Install / update",24,587,170,Install);ButtonAt("Undo last install",210,587,170,Undo);ButtonAt("Verify package",396,587,170,()=>{Verify();SelfTest();status.Text="Package integrity and file-operation self-tests passed.";});ButtonAt("Launch Bully",582,587,170,()=>Process.Start(new ProcessStartInfo("steam://rungameid/12200"){UseShellExecute=true}));
  status=new Label{Left=24,Top=639,Width=818,Height=55,Text="Left click: melee. Q: fire / throw. Duplicate melee / fire bindings are repaired on install; read README.",ForeColor=Color.FromArgb(66,77,94)};Controls.Add(status);
 }
 string FindGame(){string g="G:\\SteamLibrary\\steamapps\\common\\Bully Scholarship Edition";return File.Exists(Path.Combine(g,"Bully.exe"))?g:Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),"Steam","steamapps","common","Bully Scholarship Edition");}
 void AddLabel(string text,int x,int y,int size){Controls.Add(new Label{Text=text,Left=x,Top=y,AutoSize=true,Font=new Font("Segoe UI",size)});}
 NumericUpDown Number(int x,int y,int low,int high,int value){var n=new NumericUpDown{Left=x,Top=y,Width=94,Minimum=low,Maximum=high,Value=Math.Max(low,Math.Min(high,value))};Controls.Add(n);return n;}
 void ButtonAt(string text,int x,int y,int w,Action action){var b=new Button{Text=text,Left=x,Top=y,Width=w,Height=36};b.Click+=(s,e)=>{try{action();}catch(Exception ex){status.Text=ex.Message;MessageBox.Show(this,ex.Message,"RescuePC Bully",MessageBoxButtons.OK,MessageBoxIcon.Warning);}};Controls.Add(b);}
 static string KeyName(int code,int action){if(action==7&&code==88)return "Mouse 5 (hold)";var names=new Dictionary<int,string>{{256,"Left mouse"},{257,"Right mouse"},{258,"Middle mouse"},{259,"Mouse 4"},{260,"Wheel up"},{261,"Wheel down"},{262,"Mouse Y−"},{263,"Mouse Y+"},{264,"Mouse X−"},{265,"Mouse X+"},{203,"Left arrow"},{205,"Right arrow"},{200,"Up arrow"},{208,"Down arrow"},{1,"Escape"},{57,"Space"},{42,"Shift"}};if(names.ContainsKey(code))return names[code];return ((Keys)MapVirtualKey((uint)code,3)).ToString();}
 void CaptureKey(object sender,KeyEventArgs e){if(e.KeyCode==Keys.Tab||e.KeyCode==Keys.Enter)return;int c=(int)MapVirtualKey((uint)e.KeyCode,4);if(c>255)c=(c&255)|128;if(c>0&&c<256){var row=bindings.CurrentRow;row.Cells[1].Value=e.KeyCode.ToString();row.Cells[2].Value=c;((Control)sender).Text=e.KeyCode.ToString();e.SuppressKeyPress=true;}}
 int[] ReadKeys(){bindings.EndEdit();int[] keys=new int[25];for(int i=0;i<25;i++){int code;string text=Convert.ToString(bindings.Rows[i].Cells[1].Value);if(!int.TryParse(text,out code))code=Convert.ToInt32(bindings.Rows[i].Cells[2].Value);if(code<0||code>265)throw new IOException("Invalid native binding for "+Actions[i]);keys[i]=code;}keys[7]=88;if(keys[6]==keys[12])throw new IOException("Melee and fire / throw must use different bindings. Use Left mouse for melee and Q for fire / throw.");return keys;}
 byte[] Profile(byte[] current,int[] keys){if(current.Length!=236)throw new IOException("Unsupported control profile. Start the game and save controls first.");var b=(byte[])current.Clone();for(int i=0;i<25;i++)Buffer.BlockCopy(BitConverter.GetBytes(keys[i]),0,b,64+4*i,4);return b;}
 byte[] RepairCombatProfile(byte[] current){
  if(current.Length!=236)throw new IOException("Unsupported control profile. Start the game and save controls first.");
  if(BitConverter.ToInt32(current,88)!=BitConverter.ToInt32(current,112))return null;
  int[] candidates={16,33,47,48};
  foreach(int candidate in candidates){
   bool used=false;for(int i=0;i<25;i++)if(BitConverter.ToInt32(current,64+4*i)==candidate)used=true;
   if(!used){var result=(byte[])current.Clone();Buffer.BlockCopy(BitConverter.GetBytes(candidate),0,result,112,4);return result;}
  }
  throw new IOException("Melee and fire / throw share a key, and Q/F/V/B are already assigned. Enable Apply the control layout and choose separate combat bindings.");
 }
 void Verify(){var entries=json.Deserialize<Dictionary<string,string>>(File.ReadAllText(Path.Combine(root,"payload-sha256.json")));foreach(var p in entries){string path=Path.GetFullPath(Path.Combine(root,p.Key));if(!path.StartsWith(Path.GetFullPath(root),StringComparison.OrdinalIgnoreCase)||!File.Exists(path)||HashFile(path)!=p.Value)throw new IOException("Package integrity failed: "+p.Key);}}
 byte[] Prompt(byte[] b){const string old="9A851D427149826EE7E58DA5EAB35EB11275456C5D4BB805EC628B8B0A3D464E",patched="12A7A7B94119C12B6EB5A3CA4B7077F6FCAE1952585B4A32972219705BD7CFED";string h=Hash(b);if(h==patched)return b;if(h!=old)throw new IOException("The HUD asset differs from the supported original. No game files changed. Restore a compatible HUD first.");var p=File.ReadAllBytes(Path.Combine(root,"Payload","Mouse5Prompt.bin"));if(p.Length!=4096)throw new IOException("Invalid prompt artwork");var result=(byte[])b.Clone();Buffer.BlockCopy(p,0,result,413088,p.Length);if(Hash(result)!=patched)throw new IOException("HUD delta verification failed");return result;}
 string CheckGame(){if(Process.GetProcessesByName("Bully").Length!=0)throw new IOException("Save and close Bully first. Setup does not close your game.");string g=Path.GetFullPath(folder.Text);if(!File.Exists(Path.Combine(g,"Bully.exe"))||HashFile(Path.Combine(g,"Bully.exe"))!=ExeHash)throw new IOException("This preview supports only the tested Steam 1.200 Bully.exe. Nothing changed.");return g;}
 void SaveRecord(string dir,InstallRecord r){File.WriteAllText(Path.Combine(dir,"install.json"),json.Serialize(r));}
 void Write(string target,byte[] bytes,string dir,InstallRecord r){var change=new FileChange{Target=target,Backup=Path.Combine(dir,"Files",r.Files.Count.ToString("D3")),Existed=File.Exists(target),After=Hash(bytes)};Directory.CreateDirectory(Path.GetDirectoryName(change.Backup));if(change.Existed)File.Copy(target,change.Backup,false);r.Files.Add(change);SaveRecord(dir,r);Directory.CreateDirectory(Path.GetDirectoryName(target));File.WriteAllBytes(target,bytes);}
 void Remove(string target,string dir,InstallRecord r){
  if(!File.Exists(target))return;
  var change=new FileChange{Target=target,Backup=Path.Combine(dir,"Files",r.Files.Count.ToString("D3")),Existed=true,After=null};
  Directory.CreateDirectory(Path.GetDirectoryName(change.Backup));File.Copy(target,change.Backup,false);r.Files.Add(change);SaveRecord(dir,r);File.Delete(target);
 }
 static string Ini(string original,string section,string key,string value){var lines=original.Replace("\r","").Split('\n').ToList();int start=lines.FindIndex(x=>x.Trim().Equals("["+section+"]",StringComparison.OrdinalIgnoreCase));if(start<0){lines.Add("["+section+"]");lines.Add(key+"="+value);}else{int end=start+1;while(end<lines.Count&&!lines[end].TrimStart().StartsWith("["))end++;int at=-1;for(int i=start+1;i<end;i++)if(lines[i].Split('=')[0].Trim().Equals(key,StringComparison.OrdinalIgnoreCase)){at=i;break;}if(at>=0)lines[at]=key+"="+value;else lines.Insert(end,key+"="+value);}return string.Join("\r\n",lines);}
 void Install(){
  Verify();string g=CheckGame();if(!File.Exists(Path.Combine(g,"dinput8.dll")))throw new IOException("An ASI loader is required. This preview contains only our custom files and does not download dependencies.");
  string hud=Path.Combine(g,"TXD","HUDElems.nft");byte[] prompt=Prompt(File.ReadAllBytes(hud));string user=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),"Bully Scholarship Edition","ControllerSettings");byte[] profile=applyControls.Checked?Profile(File.ReadAllBytes(user),ReadKeys()):(File.Exists(user)?RepairCombatProfile(File.ReadAllBytes(user)):null);
  string renderPath=Path.Combine(g,"plugins","Tyler.BullyRender.asi");if(!render.Checked&&File.Exists(renderPath)&&HashFile(renderPath)!=HashFile(Path.Combine(root,"Payload","Tyler.BullyRender.asi")))throw new IOException("The installed experimental renderer differs from this package. Back it up and disable it manually before installing without rendering.");
  string dir=Path.Combine(root,"Backups",DateTime.Now.ToString("yyyyMMdd-HHmmss")+"-"+Guid.NewGuid().ToString("N").Substring(0,6));Directory.CreateDirectory(dir);var r=new InstallRecord{Game=g};SaveRecord(dir,r);
  try{
   Write(Path.Combine(g,"plugins","Tyler.BullyFix.asi"),File.ReadAllBytes(Path.Combine(root,"Payload","Tyler.BullyFix.asi")),dir,r);
   Write(Path.Combine(g,"plugins","Tyler.BullyFix.ini"),Encoding.ASCII.GetBytes("[Controls]\r\nSprintMouseButton=5\r\n[Display]\r\nBorderless=1\r\n[Diagnostics]\r\nFrameCaptureSeconds=0\r\n"),dir,r);
   Write(hud,prompt,dir,r);
   if(render.Checked){Write(Path.Combine(g,"plugins","Tyler.BullyRender.asi"),File.ReadAllBytes(Path.Combine(root,"Payload","Tyler.BullyRender.asi")),dir,r);Write(Path.Combine(g,"plugins","Tyler.BullyRender.ini"),Encoding.ASCII.GetBytes("[Rendering]\r\nAnisotropy=16\r\nMaxMSAA=4\r\nVSync=1\r\n"),dir,r);}
   if(!render.Checked)Remove(renderPath,dir,r);
   string sp=Path.Combine(g,"SilentPatchBully.ini");if(File.Exists(Path.Combine(g,"SilentPatchBully.asi")))Write(sp,Encoding.ASCII.GetBytes(Ini(File.Exists(sp)?File.ReadAllText(sp):"","SilentPatch","FPSLimit",fps.Text)),dir,r);
   string wf=Path.Combine(g,"plugins","Bully.WidescreenFix.ini");if(File.Exists(Path.Combine(g,"plugins","Bully.WidescreenFix.asi")))Write(wf,Encoding.ASCII.GetBytes(Ini(File.Exists(wf)?File.ReadAllText(wf):"","MAIN","FOVFactor",(fov.Value/100).ToString(System.Globalization.CultureInfo.InvariantCulture))),dir,r);
   if(profile!=null)Write(user,profile,dir,r);
   using(var k=Registry.CurrentUser.CreateSubKey(RegPath)){var settings=new Dictionary<string,int>{{"RESW",(int)width.Value},{"RESH",(int)height.Value},{"AA",4},{"SHA",3},{"VS",1},{"WIN",1}};foreach(var s in settings){object old=k.GetValue(s.Key);r.Registry[s.Key]=old==null?(int?)null:Convert.ToInt32(old);r.Applied[s.Key]=s.Value;SaveRecord(dir,r);k.SetValue(s.Key,s.Value,RegistryValueKind.DWord);}}
   File.WriteAllText(Path.Combine(root,"LastInstall.txt"),dir);status.Text="Installed. Launch through Steam. Renderer: "+(render.Checked?"experimental ON":"OFF")+". Backup: "+dir;
  }catch(Exception ex){try{Rollback(r,false);}catch(Exception recovery){File.WriteAllText(Path.Combine(root,"LastInstall.txt"),dir);throw new IOException("Install failed: "+ex.Message+". Recovery needs attention: "+recovery.Message+". Backup: "+dir);}throw new IOException("Install failed and previous files were restored: "+ex.Message);}
 }
 void Rollback(InstallRecord r,bool check){
  if(check){foreach(var f in r.Files)if(f.After==null?File.Exists(f.Target):(!File.Exists(f.Target)||HashFile(f.Target)!=f.After))throw new IOException("A file changed since installation; automatic undo stopped to preserve it: "+f.Target);using(var k=Registry.CurrentUser.OpenSubKey(RegPath)){foreach(var p in r.Applied)if(k==null||!Equals(k.GetValue(p.Key),p.Value))throw new IOException("Graphics settings changed since installation. Undo stopped to preserve them.");}}
  foreach(var f in r.Files.AsEnumerable().Reverse()){if(f.Existed)File.Copy(f.Backup,f.Target,true);else if(File.Exists(f.Target))File.Delete(f.Target);}if(r.Registry.Count>0)using(var k=Registry.CurrentUser.CreateSubKey(RegPath))foreach(var s in r.Registry){if(s.Value.HasValue)k.SetValue(s.Key,s.Value.Value,RegistryValueKind.DWord);else k.DeleteValue(s.Key,false);}
 }
 void Undo(){string g=CheckGame();string p=File.ReadAllText(Path.Combine(root,"LastInstall.txt")).Trim();string prefix=Path.GetFullPath(Path.Combine(root,"Backups"))+Path.DirectorySeparatorChar;if(!Path.GetFullPath(p).StartsWith(prefix,StringComparison.OrdinalIgnoreCase))throw new IOException("Invalid backup path");var r=json.Deserialize<InstallRecord>(File.ReadAllText(Path.Combine(p,"install.json")));if(!Path.GetFullPath(r.Game).Equals(g,StringComparison.OrdinalIgnoreCase))throw new IOException("Choose the same game folder used for installation.");Rollback(r,true);File.WriteAllText(Path.Combine(p,"undone.txt"),DateTime.Now.ToString("o"));File.Delete(Path.Combine(root,"LastInstall.txt"));status.Text="Previous files and graphics settings restored. Backup retained.";}
 void SelfTest(){
  byte[] b=Enumerable.Range(0,236).Select(x=>(byte)x).ToArray();byte[] p=Profile(b,DefaultKeys);for(int i=0;i<25;i++)if(BitConverter.ToInt32(p,64+i*4)!=DefaultKeys[i])throw new Exception("Control test failed");if(!p.Take(64).SequenceEqual(b.Take(64))||!p.Skip(164).SequenceEqual(b.Skip(164)))throw new Exception("Profile preservation failed");
  byte[] shared=(byte[])p.Clone();Buffer.BlockCopy(BitConverter.GetBytes(256),0,shared,112,4);byte[] fixedProfile=RepairCombatProfile(shared);
  if(fixedProfile==null||BitConverter.ToInt32(fixedProfile,88)!=256||BitConverter.ToInt32(fixedProfile,112)!=16)throw new Exception("Shared combat repair failed");
  for(int i=0;i<236;i++)if((i<112||i>=116)&&shared[i]!=fixedProfile[i])throw new Exception("Combat repair changed unrelated profile bytes");
  if(RepairCombatProfile(p)!=null)throw new Exception("Distinct combat bindings were modified");
  Buffer.BlockCopy(BitConverter.GetBytes(16),0,shared,80,4);fixedProfile=RepairCombatProfile(shared);
  if(BitConverter.ToInt32(fixedProfile,112)!=33)throw new Exception("Combat repair reused an occupied key");
  int[] occupied={16,33,47,48};for(int i=0;i<occupied.Length;i++)Buffer.BlockCopy(BitConverter.GetBytes(occupied[i]),0,shared,64+4*i,4);
  bool rejected=false;try{RepairCombatProfile(shared);}catch(IOException){rejected=true;}if(!rejected)throw new Exception("Occupied fallback bindings were accepted");
  string removeDir=Path.Combine(root,"SelfTest-"+Guid.NewGuid().ToString("N"));Directory.CreateDirectory(removeDir);
  try{
   string target=Path.Combine(removeDir,"renderer");File.WriteAllText(target,"renderer-before");var rec=new InstallRecord();
   Remove(target,removeDir,rec);if(File.Exists(target))throw new Exception("Renderer disable failed");
   File.WriteAllText(target,"later-edit");bool stopped=false;try{Rollback(rec,true);}catch(IOException){stopped=true;}
   if(!stopped||File.ReadAllText(target)!="later-edit")throw new Exception("Renderer undo overwrote a later edit");
   File.Delete(target);Rollback(rec,true);if(File.ReadAllText(target)!="renderer-before")throw new Exception("Renderer disable rollback failed");
  }finally{Directory.Delete(removeDir,true);}
  byte[] delta=File.ReadAllBytes(Path.Combine(root,"Payload","Mouse5Prompt.bin"));if(Hash(delta)!="B7288C7ED9D420CDD722B31010E3648C595F1AE46EAA259912D8688E95C0C0F4")throw new Exception("Prompt test failed");
  string dir=Path.Combine(root,"SelfTest-"+Guid.NewGuid().ToString("N"));Directory.CreateDirectory(dir);try{string target=Path.Combine(dir,"existing"),fresh=Path.Combine(dir,"new");File.WriteAllText(target,"before");var r=new InstallRecord();Write(target,Encoding.ASCII.GetBytes("after"),dir,r);Write(fresh,new byte[]{1,2,3},dir,r);Rollback(r,true);if(File.ReadAllText(target)!="before"||File.Exists(fresh))throw new Exception("Rollback test failed");}finally{Directory.Delete(dir,true);}
 }
}
