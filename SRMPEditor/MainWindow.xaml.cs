using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using Newtonsoft.Json;
using SRMultiplayer.Models;

namespace SRMPEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public class itemEntry
    {
        public enum itemType
        {
            Slime,
            Largo,
            Plort,
            Food,
            Resource,
            Special,
            Unsorted
        }
        public int id;
        public string name;
        public string imagePath;
        public itemType type;
    }
    public partial class MainWindow : Window
    {
        public NetworkWorld world;

        public List<NetworkAccessDoorSave> accessdoors;
        public List<NetworkEntitySave> entities;
        public List<NetworkGadgetSave> gadgets;
        public List<NetworkGordoSave> gordos;
        public List<NetworkLandPlotSave> landplots;
        public List<NetworkPuzzleSlotSave> puzzleslots;
        public List<NetworkSpawnResourceSave> spawnresources;
        public List<NetworkSwitchSave> switches;
        public List<NetworkTreasurePodSave> treasurepods;

        public Dictionary<string, Player> players;
        public Dictionary<string, itemEntry> items;
        public Dictionary<string, itemEntry> itemsByName;

        public Geometry xShape;
        public MainWindow()
        {
            xShape = Geometry.Parse("M 0 0 L 8 8 M 8 0 L 0 8");
            String json = new StreamReader(Application.GetResourceStream(new Uri("pack://application:,,,/Assets/Items.json")).Stream).ReadToEnd();
            items = JsonConvert.DeserializeObject<Dictionary<string, itemEntry>>(json);

//            json = new StreamReader(Application.GetResourceStream(new Uri("pack://application:,,,/Assets/Upgrades.json")).Stream).ReadToEnd();
//            items = JsonConvert.DeserializeObject<Dictionary<string, itemEntry>>(json);
            itemsByName = new Dictionary<string, itemEntry>();
            foreach(KeyValuePair<string, itemEntry> i in items)
            {
                Console.WriteLine("Loading id " + i.Key + ": " + i.Value.name + " (" + i.Value.type + ")");
                if(!i.Value.name.Contains("[") || i.Value.name == "[Empty]")
                    itemsByName.Add(i.Value.name, i.Value);
            }

//            this.Icon = new BitmapImage(new Uri("pack://application:,,,/Assets/Sprites/Icon.png"));
            InitializeComponent();
            Console.WriteLine("Finished!");
        }
        public async Task LoadWorld(string filename)
        {
            world = new NetworkWorld();
            accessdoors = new List<NetworkAccessDoorSave>();
            entities = new List<NetworkEntitySave>();
            gadgets = new List<NetworkGadgetSave>();
            gordos = new List<NetworkGordoSave>();
            landplots = new List<NetworkLandPlotSave>();
            puzzleslots = new List<NetworkPuzzleSlotSave>();
            spawnresources = new List<NetworkSpawnResourceSave>();
            switches = new List<NetworkSwitchSave>();
            treasurepods = new List<NetworkTreasurePodSave>();

            players = new Dictionary<string, Player>();

            string dir = System.IO.Path.GetDirectoryName(filename);
            if (System.IO.Path.GetExtension(filename).ToLower() == ".dat")
            {
                if (Directory.Exists(dir + "\\Players"))
                {
                    string[] paths = Directory.GetFiles(dir + "\\Players", "*.dat");
                    if (paths.Length != 0)
                        foreach (string p in paths)
                            players.Add(System.IO.Path.GetFileNameWithoutExtension(p), LoadPlayer(p));
                    else
                        MessageBox.Show("There's no players in your save file");
                }
                else
                    MessageBox.Show("Directory Players doesn't exists");

                using (FileStream stream = new FileStream(filename, FileMode.Open, FileAccess.Read))
                {
                    IFormatter formatter = new BinaryFormatter();
                    using (BinaryReader reader = new BinaryReader(stream))
                    {
                        var worldversion = reader.ReadInt32();
                        world.WorldTime = reader.ReadDouble();
                        world.Keys = reader.ReadInt32();

                        if (worldversion >= 2)
                        {
                            int progressCount = reader.ReadInt32();
                            for (int i = 0; i < progressCount; i++)
                            {
                                world.Progress.Add(reader.ReadUInt16(), reader.ReadInt32());
                            }
                            int pediaCount = reader.ReadInt32();
                            for (int i = 0; i < pediaCount; i++)
                            {
                                world.PediaIDs.Add(reader.ReadUInt16());
                            }
                            int mapCount = reader.ReadInt32();
                            for (int i = 0; i < mapCount; i++)
                            {
                                world.UnlockedZoneMaps.Add(reader.ReadByte());
                            }
                        }
                        if (worldversion >= 3)
                        {
                            int paletteCount = reader.ReadInt32();
                            for (int i = 0; i < paletteCount; i++)
                            {
                                world.Palette.Add(reader.ReadByte(), reader.ReadUInt16());
                            }
                        }
                        if (worldversion >= 7)
                        {
                            int availBlueprintsCount = reader.ReadInt32();
                            for (int i = 0; i < availBlueprintsCount; i++)
                            {
                                world.GadgetsModel.availBlueprints.Add(reader.ReadUInt16());
                            }
                            int blueprintLockDataCount = reader.ReadInt32();
                            for (int i = 0; i < blueprintLockDataCount; i++)
                            {
                                world.GadgetsModel.blueprintLockData.Add(reader.ReadUInt16(), new BlueprintLockData()
                                {
                                    lockedUntil = reader.ReadDouble(),
                                    timedLock = reader.ReadBoolean()
                                });
                            }
                            int blueprintsCount = reader.ReadInt32();
                            for (int i = 0; i < blueprintsCount; i++)
                            {
                                world.GadgetsModel.blueprints.Add(reader.ReadUInt16());
                            }
                            int craftMatCountsCount = reader.ReadInt32();
                            for (int i = 0; i < craftMatCountsCount; i++)
                            {
                                world.GadgetsModel.craftMatCounts.Add(reader.ReadUInt16(), reader.ReadInt32());
                            }
                            int gadgetsCount = reader.ReadInt32();
                            for (int i = 0; i < gadgetsCount; i++)
                            {
                                world.GadgetsModel.gadgets.Add(reader.ReadUInt16(), reader.ReadInt32());
                            }
                            int placedGadgetCountsCount = reader.ReadInt32();
                            for (int i = 0; i < placedGadgetCountsCount; i++)
                            {
                                world.GadgetsModel.placedGadgetCounts.Add(reader.ReadUInt16(), reader.ReadInt32());
                            }
                            int registeredBlueprintsCount = reader.ReadInt32();
                            for (int i = 0; i < registeredBlueprintsCount; i++)
                            {
                                world.GadgetsModel.registeredBlueprints.Add(reader.ReadUInt16());
                            }
                        }

                        var doorCount = reader.ReadInt32();
                        for (int i = 0; i < doorCount; i++)
                        {
                            var id = reader.ReadString();
                            var pos = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                            var model = (NetworkAccessDoorModel)formatter.Deserialize(stream);

                            var door = new NetworkAccessDoorSave();
                            door.x = pos.x;
                            door.y = pos.y;
                            door.z = pos.z;
                            door.id = id;
                            door.Model = model;
                            accessdoors.Add(door);
                        }
                        var gordoCount = reader.ReadInt32();
                        for (int i = 0; i < gordoCount; i++)
                        {
                            var id = reader.ReadString();
                            var pos = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                            var model = (NetworkGordoModel)formatter.Deserialize(stream);

                            var gordo = new NetworkGordoSave();
                            gordo.x = pos.x;
                            gordo.y = pos.y;
                            gordo.z = pos.z;
                            gordo.id = id;
                            gordo.Model = model;
                            gordos.Add(gordo);
                        }
                        var plotCount = reader.ReadInt32();
                        for (int i = 0; i < plotCount; i++)
                        {
                            var id = reader.ReadString();
                            var pos = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                            var model = (NetworkLandPlotModel)formatter.Deserialize(stream);

                            var plot = new NetworkLandPlotSave();
                            plot.x = pos.x;
                            plot.x = pos.y;
                            plot.x = pos.z;
                            plot.id = id;
                            plot.Model = model;
                            landplots.Add(plot);
                        }
                        var resourceCount = reader.ReadInt32();
                        for (int i = 0; i < resourceCount; i++)
                        {
                            var pos = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                            var model = (NetworkSpawnResourceModel)formatter.Deserialize(stream);


                            var spawnResource = new NetworkSpawnResourceSave();
                            spawnResource.x = pos.x;
                            spawnResource.y = pos.y;
                            spawnResource.z = pos.z;
                            spawnResource.Model = model;
                            spawnresources.Add(spawnResource);
                        }
                        var podCount = reader.ReadInt32();
                        for (int i = 0; i < podCount; i++)
                        {
                            var id = reader.ReadString();
                            var pos = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                            var model = (NetworkTreasurePodModel)formatter.Deserialize(stream);


                            var pod = new NetworkTreasurePodSave();
                            pod.x = pos.x;
                            pod.y = pos.y;
                            pod.z = pos.z;
                            pod.id = id;
                            pod.Model = model;
                            treasurepods.Add(pod);
                        }
                        var EntityCount = reader.ReadInt32();
                        for (int i = 0; i < EntityCount; i++)
                        {
                            var id = reader.ReadInt32();
                            var pos = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                            var rot = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                            var ident = reader.ReadUInt16();
                            var regionset = reader.ReadByte();


                            var entity = new NetworkEntitySave();

                            entity.ID = id;
                            entity.Ident = ident;
                            entity.RegionSet = regionset;

                            entity.posx = pos.x;
                            entity.posy = pos.y;
                            entity.posz = pos.z;
                            entity.rotx = rot.x;
                            entity.roty = rot.y;
                            entity.rotz = rot.z;

                            if (reader.ReadBoolean()) //Animal
                            {
                                entity.AnimalModel = (NetworkAnimalModel)formatter.Deserialize(stream);
                            }
                            if (reader.ReadBoolean()) // Plort
                            {
                                entity.PlortModel = (NetworkPlortModel)formatter.Deserialize(stream);
                            }
                            if (reader.ReadBoolean()) //Produce
                            {
                                entity.ProduceModel = (NetworkProduceModel)formatter.Deserialize(stream);
                            }
                            if (reader.ReadBoolean()) //Slime
                            {
                                entity.SlimeModel = (NetworkSlimeModel)formatter.Deserialize(stream);
                            }
                            entities.Add(entity);
                        }
                        if (worldversion >= 4)
                        {
                            int switchCount = reader.ReadInt32();
                            for (int i = 0; i < switchCount; i++)
                            {
                                var id = reader.ReadString();
                                var pos = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                                var state = reader.ReadByte();


                                var net = new NetworkSwitchSave();
                                net.x = pos.x;
                                net.y = pos.y;
                                net.z = pos.z;
                                net.id = id;
                                net.Model = new NetworkMasterSwitchModel() { state = state };
                                switches.Add(net);
                            }

                            int puzzleCount = reader.ReadInt32();
                            for (int i = 0; i < puzzleCount; i++)
                            {
                                var id = reader.ReadString();
                                var pos = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                                var state = reader.ReadBoolean();


                                var net = new NetworkPuzzleSlotSave();
                                net.x = pos.x;
                                net.y = pos.y;
                                net.z = pos.z;
                                net.id = id;
                                net.Model = new NetworkPuzzleSlotModel() { filled = state };
                                puzzleslots.Add(net);
                            }
                        }
                        if (worldversion >= 5)
                        {
                            world.Seed = reader.ReadSingle();
                            int saturationCount = reader.ReadInt32();
                            for (int i = 0; i < saturationCount; i++)
                            {
                                world.Saturation.Add(reader.ReadUInt16(), reader.ReadSingle());
                            }
                        }
                        if (worldversion >= 6)
                        {
                            int gadgetCount = reader.ReadInt32();
                            for (int i = 0; i < gadgetCount; i++)
                            {
                                var id = reader.ReadString();
                                var pos = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());


                                var net = new NetworkGadgetSave();
                                net.x = pos.x;
                                net.y = pos.y;
                                net.z = pos.z;
                                net.id = id;
                                net.r = reader.ReadSingle();
                                net.Ident = reader.ReadUInt16();

                                if (reader.ReadBoolean()) net.Drone = (NetworkDroneModel)formatter.Deserialize(stream);
                                if (reader.ReadBoolean()) net.EchoNet = (NetworkEchoNetModel)formatter.Deserialize(stream);
                                if (reader.ReadBoolean()) net.Extractor = (NetworkExtractorModel)formatter.Deserialize(stream);
                                if (reader.ReadBoolean()) net.Snare = (NetworkSnareModel)formatter.Deserialize(stream);
                                if (reader.ReadBoolean()) net.WarpDepot = (NetworkWarpDepotModel)formatter.Deserialize(stream);

                                gadgets.Add(net);
                            }
                        }
                    }
                }
            } else
            {

                world = JsonConvert.DeserializeObject<NetworkWorld>(File.ReadAllText(filename));

                if (File.Exists(dir + "\\accessdoors.json"))
                    accessdoors = JsonConvert.DeserializeObject<List<NetworkAccessDoorSave>>(File.ReadAllText(dir + "\\accessdoors.json"));
                else
                {
                    accessdoors = new List<NetworkAccessDoorSave>();
                    MessageBox.Show("File accessdoors.json doesn't exists", "SRMPEditor");
                }
                if (File.Exists(dir + "\\entities.json"))
                    entities = JsonConvert.DeserializeObject<List<NetworkEntitySave>>(File.ReadAllText(dir + "\\entities.json"));
                else
                {
                    entities = new List<NetworkEntitySave>();
                    MessageBox.Show("File entities.json doesn't exists", "SRMPEditor");
                }
                if (File.Exists(dir + "\\gadgets.json"))
                    gadgets = JsonConvert.DeserializeObject<List<NetworkGadgetSave>>(File.ReadAllText(dir + "\\gadgets.json"));
                else
                {
                    gadgets = new List<NetworkGadgetSave>();
                    MessageBox.Show("File gadgets.json doesn't exists", "SRMPEditor");
                }
                if (File.Exists(dir + "\\gordos.json"))
                    gordos = JsonConvert.DeserializeObject<List<NetworkGordoSave>>(File.ReadAllText(dir + "\\gordos.json"));
                else
                {
                    gordos = new List<NetworkGordoSave>();
                    MessageBox.Show("File gordos.json doesn't exists", "SRMPEditor");
                }
                if (File.Exists(dir + "\\landplots.json"))
                    landplots = JsonConvert.DeserializeObject<List<NetworkLandPlotSave>>(File.ReadAllText(dir + "\\landplots.json"));
                else
                {
                    landplots = new List<NetworkLandPlotSave>();
                    MessageBox.Show("File landplots.json doesn't exists", "SRMPEditor");
                }
                if (File.Exists(dir + "\\puzzleslots.json"))
                    puzzleslots = JsonConvert.DeserializeObject<List<NetworkPuzzleSlotSave>>(File.ReadAllText(dir + "\\puzzleslots.json"));
                else
                {
                    puzzleslots = new List<NetworkPuzzleSlotSave>();
                    MessageBox.Show("File puzzleslots.json doesn't exists", "SRMPEditor");
                }
                if (File.Exists(dir + "\\spawnresources.json"))
                    spawnresources = JsonConvert.DeserializeObject<List<NetworkSpawnResourceSave>>(File.ReadAllText(dir + "\\spawnresources.json"));
                else
                {
                    spawnresources = new List<NetworkSpawnResourceSave>();
                    MessageBox.Show("File spawnresources.json doesn't exists", "SRMPEditor");
                }
                if (File.Exists(dir + "\\switches.json"))
                    switches = JsonConvert.DeserializeObject<List<NetworkSwitchSave>>(File.ReadAllText(dir + "\\switches.json"));
                else
                {
                    switches = new List<NetworkSwitchSave>();
                    MessageBox.Show("File switches.json doesn't exists", "SRMPEditor");
                }
                if (File.Exists(dir + "\\treasurepods.json"))
                    treasurepods = JsonConvert.DeserializeObject<List<NetworkTreasurePodSave>>(File.ReadAllText(dir + "\\treasurepods.json"));
                else
                {
                    treasurepods = new List<NetworkTreasurePodSave>();
                    MessageBox.Show("File treasurepods.json doesn't exists", "SRMPEditor");
                }
                if (Directory.Exists(dir + "\\Players"))
                {
                    string[] paths = Directory.GetFiles(dir + "\\Players", "*.json");
                    if (paths.Length != 0)
                        foreach (string p in paths)
                            players.Add(System.IO.Path.GetFileNameWithoutExtension(p), LoadPlayer(p));
                    else
                        MessageBox.Show("There's no players in your save file", "SRMPEditor");
                }
                else
                {
                    players = new Dictionary<string, Player>();
                    MessageBox.Show("Directory Players doesn't exists", "SRMPEditor");
                }
            }
                this.Content = await createEditorAsync();
                await Task.Delay(1);
                DoubleAnimation animShow = new DoubleAnimation { From = 0, To = 1, Duration = TimeSpan.FromMilliseconds(100) };
                ((Grid)this.Content).BeginAnimation(OpacityProperty, animShow);
        }
        public Player LoadPlayer(string filename) 
        {
            Player player = new Player();
            if(System.IO.Path.GetExtension(filename).ToLower() == ".dat") {
                using (FileStream stream = new FileStream(filename, FileMode.Open, FileAccess.Read))
                {
                    IFormatter formatter = new BinaryFormatter();
                    using (BinaryReader reader = new BinaryReader(stream))
                    {
                        var pos = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                        player.x = pos.x;
                        player.y = pos.y;
                        player.z = pos.z;
                        player.r = reader.ReadSingle();
                        player.Model = (NetworkPlayerModel)formatter.Deserialize(stream);
                    }
                }
            } else
                player = JsonConvert.DeserializeObject<Player>(File.ReadAllText(filename));
            if (player.Model == null) player.Model = new NetworkPlayerModel();
            if (player.Model.ammoDict == null) player.Model.ammoDict = new Dictionary<byte, NetworkAmmoModel>();
            if (player.Model.ammoDict[0] == null) player.Model.ammoDict[0] = new NetworkAmmoModel();
            if (player.Model.ammoDict[0].slots == null) player.Model.ammoDict[0].slots = new Slot[5];
            if (player.Model.ammoDict[1] == null) player.Model.ammoDict[1] = new NetworkAmmoModel();
            if (player.Model.ammoDict[1].slots == null) player.Model.ammoDict[1].slots = new Slot[3];
            if (player.Model.upgrades == null) player.Model.upgrades = new List<byte>();

            return player;
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(-1);
        }

        private void zButton_click(object sender, RoutedEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            {
//                MessageBox.Show(items[new Random().Next(-1, items.Values.Count - 2).ToString()].name);
                MessageBox.Show(items[Math.Round(((DateTime.Today.Day * DateTime.Today.Month * DateTime.Today.Year * Math.PI) % (items.Values.Count + 1) - 2)).ToString()].name, "Item of the day");
                return;
            }
            OpenFileDialog fd = new OpenFileDialog { Filter = "SRMP World Data (*.json)|*.json|Old SRMP World Data (*.dat)|*.dat", DefaultExt = ".json", RestoreDirectory = true, Title = "Load SRMP World" };
            Nullable<bool> result = fd.ShowDialog();
            if (result == true)
            {

                DoubleAnimation anim = new DoubleAnimation { To = 0, Duration = TimeSpan.FromMilliseconds(100) };
                anim.Completed += async (s, e1) => {
                    await Task.Delay(16);
                    this.Title = "SRMPEditor | " + fd.FileName;
                    LoadWorld(fd.FileName);
                };
                ((Grid)this.Content).BeginAnimation(OpacityProperty, anim);
            }
        }
        private void zButton_click_1(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog { Filter = "Old SRMP Player Data (*.dat)|*.dat", DefaultExt = ".dat", RestoreDirectory = true, Title = "Choose Old SRMP Player to convert" };
            Nullable<bool> result = fd.ShowDialog();
            if (result == true)
            {
                Player convertedPlayer = LoadPlayer(fd.FileName);
                SaveFileDialog sd = new SaveFileDialog { DefaultExt = ".json", Filter = "New SRMP Player Data (*.json)|*.json", RestoreDirectory = true, Title = "Save Converted SRMP Player", FileName = System.IO.Path.GetFileNameWithoutExtension(fd.FileName) + ".json" };
                Nullable<bool> dResult = sd.ShowDialog();
                if (dResult == true)
                    File.WriteAllText(sd.FileName, JsonConvert.SerializeObject(convertedPlayer));
                else
                    Console.WriteLine("Operation aborted.");
            }
        }
        public void save()
        {
            SaveFileDialog sd = new SaveFileDialog { DefaultExt = ".json", Filter = "SRMP World Data (*.json)|*.json", RestoreDirectory = true, Title = "Save SRMP World (Multiple files)", FileName = "world.json" };
            Nullable<bool> result = sd.ShowDialog();
            if (result == true) {
            string path = System.IO.Path.GetDirectoryName(sd.FileName);
                File.WriteAllText(path + "\\world.json", JsonConvert.SerializeObject(world));
                File.WriteAllText(path + "\\accessdoors.json", JsonConvert.SerializeObject(accessdoors));
                File.WriteAllText(path + "\\entities.json", JsonConvert.SerializeObject(entities));
                File.WriteAllText(path + "\\gadgets.json", JsonConvert.SerializeObject(gadgets));
                File.WriteAllText(path + "\\gordos.json", JsonConvert.SerializeObject(gordos));
                File.WriteAllText(path + "\\landplots.json", JsonConvert.SerializeObject(landplots));
                File.WriteAllText(path + "\\puzzleslots.json", JsonConvert.SerializeObject(puzzleslots));
                File.WriteAllText(path + "\\spawnresources.json", JsonConvert.SerializeObject(spawnresources));
                File.WriteAllText(path + "\\switches.json", JsonConvert.SerializeObject(switches));
                File.WriteAllText(path + "\\treasurepods.json", JsonConvert.SerializeObject(treasurepods));
                if (!Directory.Exists(path + "\\Players")) Directory.CreateDirectory(path + "\\Players");
                foreach (KeyValuePair<string, Player> p in players)
                    File.WriteAllText(path + "\\Players\\" + p.Key + ".json", JsonConvert.SerializeObject(p.Value));
            } else
                Console.WriteLine("Operation aborted.");
        }

        public ScrollViewer editorArea;

        public async Task<Grid> createEditorAsync()
        {
            Grid grid = new Grid { Opacity = 0 };
            Grid main = new Grid { Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0)) };
//            StackPanel menu;
            grid.Children.Add(main);
            main.RowDefinitions.Add(new RowDefinition { Height = new GridLength(24) });
            main.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            Grid workspace = new Grid { };
            Grid.SetRow(workspace, 1);
            main.Children.Add(workspace);
            
            workspace.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(240), MaxWidth = 560, MinWidth = 160 });
            Grid sidebar = new Grid { Background = new SolidColorBrush(Color.FromRgb(20, 20, 20)) };

            TreeView tree = new TreeView { Background = new SolidColorBrush(Color.FromRgb(20,20,20)), Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)), BorderThickness = new Thickness(0), FontFamily = new FontFamily("Bahnschrift"), FontWeight = FontWeights.Light };
            TreeViewItem worldItem = new TreeViewItem { Header = "World", Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)) };

            worldItem.MouseDoubleClick += (s1, e1) =>
            {
                if (e1.RightButton != MouseButtonState.Released) return;
                DoubleAnimation anim = new DoubleAnimation { To = 0, Duration = TimeSpan.FromMilliseconds(100) };
                anim.Completed += async (s, e) =>
                {
                    editorArea.Content = await createWorldEditor();

                    DoubleAnimation animShow = new DoubleAnimation { To = 1, Duration = TimeSpan.FromMilliseconds(100) };
                    editorArea.BeginAnimation(OpacityProperty, animShow);
                };
                editorArea.BeginAnimation(OpacityProperty, anim);
            };

            TreeViewItem accessdoorsItem = new TreeViewItem { Header = "Doors", Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)) };
            TreeViewItem entitiesItem = new TreeViewItem { Header = "Entities", Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)) };
            TreeViewItem gadgetsItem = new TreeViewItem { Header = "Gadgets", Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)) };
            TreeViewItem gordosItem = new TreeViewItem { Header = "Gordos", Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)) };
            TreeViewItem landplotsItem = new TreeViewItem { Header = "Plots", Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)) };
            TreeViewItem puzzleslotsItem = new TreeViewItem { Header = "Puzzles", Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)) };
            TreeViewItem spawnresourcesItem = new TreeViewItem { Header = "Resources", Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)) };
            TreeViewItem switchesItem = new TreeViewItem { Header = "Switches", Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)) };
            TreeViewItem treasurepodsItem = new TreeViewItem { Header = "Treasure Pods", Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)) };
            TreeViewItem playersItem = new TreeViewItem { Header = "Players", Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)) };

            playersItem.MouseRightButtonUp += (s1, e1) =>
            {
                if(s1 == e1.Source)
                {
                    ((TreeViewItem)s1).IsSelected = true;
                    ContextMenu menu = new ContextMenu();
                    MenuItem add = new MenuItem { Header = "Add Player" };
                    add.Click += (s, e) =>
                    {
                        OpenFileDialog fd = new OpenFileDialog { Filter = "SRMP Player Data (*.json)|*.json|Old SRMP Player Data (*.dat)|*.dat", DefaultExt = ".dat", RestoreDirectory = true, Title = "Add SRMP Player" };
                        Nullable<bool> result = fd.ShowDialog();
                        if (result == true)
                        {
                            if (!players.Keys.Contains(System.IO.Path.GetFileNameWithoutExtension(fd.FileName)))
                            {
                                players.Add(System.IO.Path.GetFileNameWithoutExtension(fd.FileName), LoadPlayer(fd.FileName));

                                TreeViewItem item = new TreeViewItem { Header = System.IO.Path.GetFileNameWithoutExtension(fd.FileName), Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)) };
                                item.MouseDoubleClick += (s2, e2) =>
                                {
                                    if (e2.RightButton != MouseButtonState.Released) return;
                                    DoubleAnimation anim = new DoubleAnimation { To = 0, Duration = TimeSpan.FromMilliseconds(100) };
                                    anim.Completed += async (s3, e3) =>
                                    {
                                        editorArea.Content = await createPlayerEditor(players[item.Header.ToString()], item.Header.ToString());
                                        await Task.Delay(1);
                                        DoubleAnimation animShow = new DoubleAnimation { To = 1, Duration = TimeSpan.FromMilliseconds(100) };
                                        editorArea.BeginAnimation(OpacityProperty, animShow);
                                    };
                                    editorArea.BeginAnimation(OpacityProperty, anim);
                                };
                                item.MouseRightButtonUp += (s2, e2) =>
                                {
                                    ((TreeViewItem)s2).IsSelected = true;
                                    ContextMenu playerMenu = new ContextMenu();
                                    MenuItem rename = new MenuItem { Header = "Rename Player" };
                                    rename.Click += (s3, e3) =>
                                    {
                                        Window dialog = new Window { Width = 320, Height = 200, Title = "Rename Player", ResizeMode = ResizeMode.NoResize, Left = SystemParameters.PrimaryScreenWidth / 2 - 160, Top = SystemParameters.PrimaryScreenHeight / 2 - 100 };
                                        Grid dialogContent = new Grid { Background = new SolidColorBrush(Color.FromRgb(24, 24, 24)) };
                                        dialog.Content = dialogContent;
                                        dialogContent.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.85, GridUnitType.Star) });
                                        dialogContent.RowDefinitions.Add(new RowDefinition { Height = new GridLength(40, GridUnitType.Pixel) });
                                        dialogContent.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                                        TextBlock text = new TextBlock { Text = "Enter username", FontWeight = FontWeights.Light, FontSize = 22, Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)), HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
                                        dialogContent.Children.Add(text);
                                        zEdit username = new zEdit { Margin = new Thickness(24, 0, 24, 0), Height = 32, Text = ((string)item.Header) };
                                        Grid.SetRow(username, 1);
                                        dialogContent.Children.Add(username);
                                        StackPanel buttons = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
                                        zButton ok = new zButton { Text = "OK", Width = 96, Height = 24, Margin = new Thickness(8) };
                                        ok.click += (s4, e4) =>
                                        {
                                            dialog.DialogResult = true;
                                            dialog.Close();
                                        };
                                        zButton cancel = new zButton { Text = "Cancel", Width = 96, Height = 24, Margin = new Thickness(8) };
                                        cancel.click += (s4, e4) =>
                                        {
                                            dialog.DialogResult = false;
                                            dialog.Close();
                                        };
                                        buttons.Children.Add(ok);
                                        buttons.Children.Add(cancel);
                                        Grid.SetRow(buttons, 2);
                                        dialogContent.Children.Add(buttons);
                                        if (dialog.ShowDialog() == true && dialog.DialogResult == true)
                                        {
                                            if (username.Text == "") {
                                                MessageBox.Show("Can't create player without username", "SRMPEditor");
                                                return;
                                            }
                                            item.Header = username.Text;
                                        }
                                    };
                                    MenuItem delete = new MenuItem { Header = "Delete Player" };
                                    delete.Click += (s3, e3) =>
                                    {
                                        Window dialog = new Window { Width = 320, Height = 200, Title = "Delete Player", ResizeMode = ResizeMode.NoResize, Left = SystemParameters.PrimaryScreenWidth / 2 - 160, Top = SystemParameters.PrimaryScreenHeight / 2 - 100 };
                                        Grid dialogContent = new Grid { Background = new SolidColorBrush(Color.FromRgb(24, 24, 24)) };
                                        dialog.Content = dialogContent;
                                        dialogContent.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1.5, GridUnitType.Star) });
                                        dialogContent.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                                        TextBlock text = new TextBlock { Text = "Are you sure you want\nto delete this player's data?", FontWeight = FontWeights.Light, FontSize = 22, Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)), HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, TextAlignment = TextAlignment.Center };
                                        dialogContent.Children.Add(text);
                                        StackPanel buttons = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
                                        zButton yes = new zButton { Text = "Yes", Width = 96, Height = 24, Margin = new Thickness(8) };
                                        yes.click += (s4, e4) =>
                                        {
                                            dialog.DialogResult = true;
                                            dialog.Close();
                                        };
                                        zButton no = new zButton { Text = "No", Width = 96, Height = 24, Margin = new Thickness(8) };
                                        no.click += (s4, e4) =>
                                        {
                                            dialog.DialogResult = false;
                                            dialog.Close();
                                        };
                                        buttons.Children.Add(yes);
                                        buttons.Children.Add(no);
                                        Grid.SetRow(buttons, 1);
                                        dialogContent.Children.Add(buttons);
                                        if (dialog.ShowDialog() == true && dialog.DialogResult == true)
                                        {
                                            playersItem.Items.Remove(item);
                                            players.Remove(item.Header.ToString());
                                        }
                                    };
                                    playerMenu.Items.Add(rename);
                                    playerMenu.Items.Add(delete);
                                    playerMenu.PlacementTarget = item;
                                    playerMenu.Background = new SolidColorBrush(Color.FromRgb(20, 20, 20));
                                    playerMenu.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                                    playerMenu.BorderThickness = new Thickness(0);
                                    playerMenu.IsOpen = true;
                                };
                                playersItem.Items.Add(item);
                            }
                            else
                                MessageBox.Show("This player already exists", "SRMPEditor"); // change on "override this player data?"
                        }
                    };
                    MenuItem create = new MenuItem { Header = "Create Player" };
                    create.Click += (s, e) =>
                    {
                        Window dialog = new Window { Width = 320, Height = 200, Title = "Create a New Player", ResizeMode = ResizeMode.NoResize, Left = SystemParameters.PrimaryScreenWidth / 2 - 160, Top = SystemParameters.PrimaryScreenHeight / 2 - 100 };
                        Grid dialogContent = new Grid { Background = new SolidColorBrush(Color.FromRgb(24, 24, 24)) };
                        dialog.Content = dialogContent;
                        dialogContent.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.85, GridUnitType.Star) });
                        dialogContent.RowDefinitions.Add(new RowDefinition { Height = new GridLength(40, GridUnitType.Pixel) });
                        dialogContent.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                        TextBlock text = new TextBlock { Text = "Enter username", FontWeight = FontWeights.Light, FontSize = 22, Foreground = new SolidColorBrush(Color.FromRgb(255,255,255)), HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
                        dialogContent.Children.Add(text);
                        zEdit username = new zEdit { Margin = new Thickness(24, 0, 24, 0), Height = 32, Text = "" };
                        Grid.SetRow(username, 1);
                        dialogContent.Children.Add(username);
                        StackPanel buttons = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
                        zButton ok = new zButton { Text = "OK", Width = 96, Height = 24, Margin = new Thickness(8) };
                        ok.click += (s2, e2) =>
                        {
                            dialog.DialogResult = true;
                            dialog.Close();
                        };
                        zButton cancel = new zButton { Text = "Cancel", Width = 96, Height = 24, Margin = new Thickness(8) };
                        cancel.click += (s2, e2) =>
                        {
                            dialog.DialogResult = false;
                            dialog.Close();
                        };
                        buttons.Children.Add(ok);
                        buttons.Children.Add(cancel);
                        Grid.SetRow(buttons, 2);
                        dialogContent.Children.Add(buttons);
                        if (dialog.ShowDialog() == true && dialog.DialogResult == true)
                        {
                            if (username.Text == "") {
                                MessageBox.Show("Can't create player without username", "SRMPEditor");
                                return;
                            }
                            Player player = new Player();
                            players.Add(username.Text, player);

                            TreeViewItem item = new TreeViewItem { Header = username.Text, Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)) };
                            item.MouseDoubleClick += (s2, e2) =>
                            {
                                if (e2.RightButton != MouseButtonState.Released) return;
                                DoubleAnimation anim = new DoubleAnimation { To = 0, Duration = TimeSpan.FromMilliseconds(100) };
                                anim.Completed += async (s3, e3) =>
                                {
                                    editorArea.Content = await createPlayerEditor(players[item.Header.ToString()], item.Header.ToString());
                                    await Task.Delay(1);
                                    DoubleAnimation animShow = new DoubleAnimation { To = 1, Duration = TimeSpan.FromMilliseconds(100) };
                                    editorArea.BeginAnimation(OpacityProperty, animShow);
                                };
                                editorArea.BeginAnimation(OpacityProperty, anim);
                            };

                            item.MouseRightButtonUp += (s2, e2) =>
                            {
                                ((TreeViewItem)s2).IsSelected = true;
                                ContextMenu playerMenu = new ContextMenu();
                                MenuItem rename = new MenuItem { Header = "Rename Player" };
                                rename.Click += (s3, e3) =>
                                {
                                    Window dialog1 = new Window { Width = 320, Height = 200, Title = "Rename Player", ResizeMode = ResizeMode.NoResize, Left = SystemParameters.PrimaryScreenWidth / 2 - 160, Top = SystemParameters.PrimaryScreenHeight / 2 - 100 };
                                    Grid dialogContent1 = new Grid { Background = new SolidColorBrush(Color.FromRgb(24, 24, 24)) };
                                    dialog1.Content = dialogContent1;
                                    dialogContent1.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.85, GridUnitType.Star) });
                                    dialogContent1.RowDefinitions.Add(new RowDefinition { Height = new GridLength(40, GridUnitType.Pixel) });
                                    dialogContent1.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                                    TextBlock text1 = new TextBlock { Text = "Enter username", FontWeight = FontWeights.Light, FontSize = 22, Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)), HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
                                    dialogContent1.Children.Add(text1);
                                    zEdit username1 = new zEdit { Margin = new Thickness(24, 0, 24, 0), Height = 32, Text = ((string)item.Header) };
                                    Grid.SetRow(username1, 1);
                                    dialogContent1.Children.Add(username1);
                                    StackPanel buttons1 = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
                                    zButton ok1 = new zButton { Text = "OK", Width = 96, Height = 24, Margin = new Thickness(8) };
                                    ok1.click += (s4, e4) =>
                                    {
                                        dialog1.DialogResult = true;
                                        dialog1.Close();
                                    };
                                    zButton cancel1 = new zButton { Text = "Cancel", Width = 96, Height = 24, Margin = new Thickness(8) };
                                    cancel1.click += (s4, e4) =>
                                    {
                                        dialog1.DialogResult = false;
                                        dialog1.Close();
                                    };
                                    buttons1.Children.Add(ok1);
                                    buttons1.Children.Add(cancel1);
                                    Grid.SetRow(buttons1, 2);
                                    dialogContent1.Children.Add(buttons1);
                                    if (dialog1.ShowDialog() == true && dialog1.DialogResult == true)
                                    {
                                        if (username1.Text == "") {
                                            MessageBox.Show("Can't create player without username", "SRMPEditor");
                                            return;
                                        }
                                        item.Header = username1.Text;
                                    }
                                };
                                MenuItem delete = new MenuItem { Header = "Delete Player" };
                                delete.Click += (s3, e3) =>
                                {
                                    Window dialog1 = new Window { Width = 320, Height = 200, Title = "Delete Player", ResizeMode = ResizeMode.NoResize, Left = SystemParameters.PrimaryScreenWidth / 2 - 160, Top = SystemParameters.PrimaryScreenHeight / 2 - 100 };
                                    Grid dialogContent1 = new Grid { Background = new SolidColorBrush(Color.FromRgb(24, 24, 24)) };
                                    dialog1.Content = dialogContent1;
                                    dialogContent1.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1.5, GridUnitType.Star) });
                                    dialogContent1.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                                    TextBlock text1 = new TextBlock { Text = "Are you sure you want\nto delete this player's data?", FontWeight = FontWeights.Light, FontSize = 22, Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)), HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, TextAlignment = TextAlignment.Center };
                                    dialogContent1.Children.Add(text1);
                                    StackPanel buttons1 = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
                                    zButton yes = new zButton { Text = "Yes", Width = 96, Height = 24, Margin = new Thickness(8) };
                                    yes.click += (s4, e4) =>
                                    {
                                        dialog1.DialogResult = true;
                                        dialog1.Close();
                                    };
                                    zButton no = new zButton { Text = "No", Width = 96, Height = 24, Margin = new Thickness(8) };
                                    no.click += (s4, e4) =>
                                    {
                                        dialog1.DialogResult = false;
                                        dialog1.Close();
                                    };
                                    buttons1.Children.Add(yes);
                                    buttons1.Children.Add(no);
                                    Grid.SetRow(buttons1, 1);
                                    dialogContent1.Children.Add(buttons1);
                                    if (dialog1.ShowDialog() == true && dialog1.DialogResult == true)
                                    {
                                        playersItem.Items.Remove(item);
                                        players.Remove(item.Header.ToString());
                                    }
                                };
                                playerMenu.Items.Add(rename);
                                playerMenu.Items.Add(delete);
                                playerMenu.PlacementTarget = item;
                                playerMenu.Background = new SolidColorBrush(Color.FromRgb(20, 20, 20));
                                playerMenu.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                                playerMenu.BorderThickness = new Thickness(0);
                                playerMenu.IsOpen = true;
                            };
                            playersItem.Items.Add(item);
                        }
                    }; 
                        menu.Items.Add(add);
                        menu.Items.Add(create);
                        menu.PlacementTarget = playersItem;
                        menu.Background = new SolidColorBrush(Color.FromRgb(20, 20, 20));
                        menu.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                        menu.BorderThickness = new Thickness(0);
                        menu.IsOpen = true;
                }
            };
            foreach( NetworkAccessDoorSave net in accessdoors )
            {
                TreeViewItem item = new TreeViewItem { Header = net.id, Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)) };
                accessdoorsItem.Items.Add(item);
            }
            foreach (NetworkEntitySave net in entities)
            {
                TreeViewItem item = new TreeViewItem { Header = net.ID, Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)) };
                entitiesItem.Items.Add(item);
            }
            foreach (NetworkGadgetSave net in gadgets)
            {
                TreeViewItem item = new TreeViewItem { Header = net.id, Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)) };
                gadgetsItem.Items.Add(item);
            }
            foreach (NetworkGordoSave net in gordos)
            {
                TreeViewItem item = new TreeViewItem { Header = net.id, Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)) };
                gordosItem.Items.Add(item);
            }
            foreach (NetworkLandPlotSave net in landplots)
            {
                TreeViewItem item = new TreeViewItem { Header = net.id, Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)) };
                landplotsItem.Items.Add(item);
            }
            foreach (NetworkPuzzleSlotSave net in puzzleslots)
            {
                TreeViewItem item = new TreeViewItem { Header = net.id, Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)) };
                puzzleslotsItem.Items.Add(item);
            }
            foreach (NetworkSpawnResourceSave net in spawnresources)
            {
                TreeViewItem item = new TreeViewItem { Header = net.x + ", " + net.y + ", " + net.z, Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)) };
                spawnresourcesItem.Items.Add(item);
            }
            foreach (NetworkSwitchSave net in switches)
            {
                TreeViewItem item = new TreeViewItem { Header = net.id, Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)) };
                switchesItem.Items.Add(item);
            }
            foreach (NetworkTreasurePodSave net in treasurepods)
            {
                TreeViewItem item = new TreeViewItem { Header = net.id, Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)) };
                treasurepodsItem.Items.Add(item);
            }
            foreach (KeyValuePair<string, Player> p in players)
            {
                TreeViewItem item = new TreeViewItem { Header = p.Key, Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)) };
                item.MouseDoubleClick += (s1, e1) =>
                {
                    if (e1.RightButton != MouseButtonState.Released) return;
                    DoubleAnimation anim = new DoubleAnimation { To = 0, Duration = TimeSpan.FromMilliseconds(100) };
                    anim.Completed += async (s, e) =>
                    {
                        editorArea.Content = await createPlayerEditor(players[item.Header.ToString()], item.Header.ToString());
                        await Task.Delay(1);
                        DoubleAnimation animShow = new DoubleAnimation { To = 1, Duration = TimeSpan.FromMilliseconds(100) };
                        editorArea.BeginAnimation(OpacityProperty, animShow);
                    };
                    editorArea.BeginAnimation(OpacityProperty, anim);
                };
                item.MouseRightButtonUp += (s2, e2) =>
                {
                    ((TreeViewItem)s2).IsSelected = true;
                    ContextMenu playerMenu = new ContextMenu();
                    MenuItem rename = new MenuItem { Header = "Rename Player" };
                    rename.Click += (s3, e3) =>
                    {
                        Window dialog = new Window { Width = 320, Height = 200, Title = "Rename Player", ResizeMode = ResizeMode.NoResize, Left = SystemParameters.PrimaryScreenWidth / 2 - 160, Top = SystemParameters.PrimaryScreenHeight / 2 - 100 };
                        Grid dialogContent = new Grid { Background = new SolidColorBrush(Color.FromRgb(24, 24, 24)) };
                        dialog.Content = dialogContent;
                        dialogContent.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.85, GridUnitType.Star) });
                        dialogContent.RowDefinitions.Add(new RowDefinition { Height = new GridLength(40, GridUnitType.Pixel) });
                        dialogContent.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                        TextBlock text = new TextBlock { Text = "Enter username", FontWeight = FontWeights.Light, FontSize = 22, Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)), HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
                        dialogContent.Children.Add(text);
                        zEdit username = new zEdit { Margin = new Thickness(24, 0, 24, 0), Height = 32, Text = ((string)item.Header) };
                        Grid.SetRow(username, 1);
                        dialogContent.Children.Add(username);
                        StackPanel buttons = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
                        zButton ok = new zButton { Text = "OK", Width = 96, Height = 24, Margin = new Thickness(8) };
                        ok.click += (s4, e4) =>
                        {
                            dialog.DialogResult = true;
                            dialog.Close();
                        };
                        zButton cancel = new zButton { Text = "Cancel", Width = 96, Height = 24, Margin = new Thickness(8) };
                        cancel.click += (s4, e4) =>
                        {
                            dialog.DialogResult = false;
                            dialog.Close();
                        };
                        buttons.Children.Add(ok);
                        buttons.Children.Add(cancel);
                        Grid.SetRow(buttons, 2);
                        dialogContent.Children.Add(buttons);
                        if (dialog.ShowDialog() == true && dialog.DialogResult == true)
                        {
                            if (username.Text == "")
                            {
                                MessageBox.Show("Can't create player without username", "SRMPEditor");
                                return;
                            }
                            item.Header = username.Text;
                        }
                    };
                    MenuItem delete = new MenuItem { Header = "Delete Player" };
                    delete.Click += (s3, e3) =>
                    {
                        Window dialog = new Window { Width = 320, Height = 200, Title = "Delete Player", ResizeMode = ResizeMode.NoResize, Left = SystemParameters.PrimaryScreenWidth / 2 - 160, Top = SystemParameters.PrimaryScreenHeight / 2 - 100 };
                        Grid dialogContent = new Grid { Background = new SolidColorBrush(Color.FromRgb(24, 24, 24)) };
                        dialog.Content = dialogContent;
                        dialogContent.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1.5, GridUnitType.Star) });
                        dialogContent.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                        TextBlock text = new TextBlock { Text = "Are you sure you want\nto delete this player's data?", FontWeight = FontWeights.Light, FontSize = 22, Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)), HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, TextAlignment = TextAlignment.Center };
                        dialogContent.Children.Add(text);
                        StackPanel buttons = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
                        zButton yes = new zButton { Text = "Yes", Width = 96, Height = 24, Margin = new Thickness(8) };
                        yes.click += (s4, e4) =>
                        {
                            dialog.DialogResult = true;
                            dialog.Close();
                        };
                        zButton no = new zButton { Text = "No", Width = 96, Height = 24, Margin = new Thickness(8) };
                        no.click += (s4, e4) =>
                        {
                            dialog.DialogResult = false;
                            dialog.Close();
                        };
                        buttons.Children.Add(yes);
                        buttons.Children.Add(no);
                        Grid.SetRow(buttons, 1);
                        dialogContent.Children.Add(buttons);
                        if (dialog.ShowDialog() == true && dialog.DialogResult == true)
                        {
                            playersItem.Items.Remove(item);
                            players.Remove(item.Header.ToString());
                        }
                    };
                    playerMenu.Items.Add(rename);
                    playerMenu.Items.Add(delete);
                    playerMenu.PlacementTarget = item;
                    playerMenu.Background = new SolidColorBrush(Color.FromRgb(20, 20, 20));
                    playerMenu.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                    playerMenu.BorderThickness = new Thickness(0);
                    playerMenu.IsOpen = true;
                };
                playersItem.Items.Add(item);
            }

            tree.Items.Add(worldItem);
            tree.Items.Add(accessdoorsItem);
            tree.Items.Add(entitiesItem);
            tree.Items.Add(gadgetsItem);
            tree.Items.Add(gordosItem);
            tree.Items.Add(landplotsItem);
            tree.Items.Add(puzzleslotsItem);
            tree.Items.Add(spawnresourcesItem);
            tree.Items.Add(switchesItem);
            tree.Items.Add(playersItem);

            sidebar.Children.Add(tree);

            workspace.Children.Add(sidebar);
            
            workspace.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            editorArea = new ScrollViewer { Background = new SolidColorBrush(Color.FromRgb(24,24,24)), Margin = new Thickness(2,0,0,0) };
            editorArea.Content = await createWorldEditor();
            Grid.SetColumn(editorArea, 1);
            workspace.Children.Add(editorArea);
            GridSplitter splitter = new GridSplitter { Width = 3, Background = new SolidColorBrush(Color.FromRgb(38, 38, 38)), HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Stretch };
            Grid.SetColumn(splitter, 1);
            workspace.Children.Add(splitter);

            StackPanel dockPanel = new StackPanel { Orientation = Orientation.Horizontal, Background = new SolidColorBrush(Color.FromRgb(20, 20, 20)) };
            main.Children.Add(dockPanel);
            zButton worldButton = new zButton { Text = "World", Width = 64, color = Color.FromRgb(20, 20, 20), colorHover = Color.FromRgb(38, 38, 38), colorClicked = Color.FromRgb(20, 20, 20), Corner = new CornerRadius(0) };
            worldButton.click += (s1, e1) =>
            {
                ContextMenu menu = new ContextMenu { FontFamily = new FontFamily("Bahnschrift"), FontWeight = FontWeights.Light };
                MenuItem open = new MenuItem { Header = "Open World" };
                open.Click += (s, e) =>
                {
                    OpenFileDialog fd = new OpenFileDialog { Filter = "SRMP World Data (*.json)|*.json|Old SRMP World Data (*.dat)|*.dat", DefaultExt = ".json", RestoreDirectory = true, Title = "Load SRMP World" };
                    Nullable<bool> result = fd.ShowDialog();
                    if (result == true)
                    {

                        DoubleAnimation anim = new DoubleAnimation { To = 0, Duration = TimeSpan.FromMilliseconds(100) };
                        anim.Completed += async (s2, e2) => {
                            await Task.Delay(16);
                            this.Title = "SRMPEditor | " + fd.FileName;
                            LoadWorld(fd.FileName);
                        };
                        ((Grid)this.Content).BeginAnimation(OpacityProperty, anim);
                    }
                };
                MenuItem saveMenuItem = new MenuItem { Header = "Save World" };
                saveMenuItem.Click += (s, e) =>
                {
                    save();
                };
                menu.Items.Add(open);
                menu.Items.Add(saveMenuItem);
                menu.PlacementTarget = worldButton;
                menu.Background = new SolidColorBrush(Color.FromRgb(20, 20, 20));
                menu.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                menu.BorderThickness = new Thickness(0);
                menu.IsOpen = true;
            };
            zButton playersButton = new zButton { Text = "Players", Width = 64, color = Color.FromRgb(20, 20, 20), colorHover = Color.FromRgb(38, 38, 38), colorClicked = Color.FromRgb(20, 20, 20), Corner = new CornerRadius(0) };
            playersButton.click += (s1, e1) =>
            {
                ContextMenu menu = new ContextMenu();
                MenuItem add = new MenuItem { Header = "Add Player" };
                add.Click += (s, e) =>
                {
                    OpenFileDialog fd = new OpenFileDialog { Filter = "SRMP Player Data (*.json)|*.json|Old SRMP Player Data (*.dat)|*.dat", DefaultExt = ".dat", RestoreDirectory = true, Title = "Add SRMP Player" };
                    Nullable<bool> result = fd.ShowDialog();
                    if (result == true)
                    {
                        if (!players.Keys.Contains(System.IO.Path.GetFileNameWithoutExtension(fd.FileName)))
                        {
                            players.Add(System.IO.Path.GetFileNameWithoutExtension(fd.FileName), LoadPlayer(fd.FileName));

                            TreeViewItem item = new TreeViewItem { Header = System.IO.Path.GetFileNameWithoutExtension(fd.FileName), Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)) };
                            item.MouseDoubleClick += (s2, e2) =>
                            {
                                if (e2.RightButton != MouseButtonState.Released) return;
                                DoubleAnimation anim = new DoubleAnimation { To = 0, Duration = TimeSpan.FromMilliseconds(100) };
                                anim.Completed += async (s3, e3) =>
                                {
                                    editorArea.Content = await createPlayerEditor(players[item.Header.ToString()], item.Header.ToString());
                                    await Task.Delay(1);
                                    DoubleAnimation animShow = new DoubleAnimation { To = 1, Duration = TimeSpan.FromMilliseconds(100) };
                                    editorArea.BeginAnimation(OpacityProperty, animShow);
                                };
                                editorArea.BeginAnimation(OpacityProperty, anim);
                            };
                            item.MouseRightButtonUp += (s2, e2) =>
                            {
                                ((TreeViewItem)s2).IsSelected = true;
                                ContextMenu playerMenu = new ContextMenu();
                                MenuItem rename = new MenuItem { Header = "Rename Player" };
                                rename.Click += (s3, e3) =>
                                {
                                    Window dialog = new Window { Width = 320, Height = 200, Title = "Rename Player", ResizeMode = ResizeMode.NoResize, Left = SystemParameters.PrimaryScreenWidth / 2 - 160, Top = SystemParameters.PrimaryScreenHeight / 2 - 100 };
                                    Grid dialogContent = new Grid { Background = new SolidColorBrush(Color.FromRgb(24, 24, 24)) };
                                    dialog.Content = dialogContent;
                                    dialogContent.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.85, GridUnitType.Star) });
                                    dialogContent.RowDefinitions.Add(new RowDefinition { Height = new GridLength(40, GridUnitType.Pixel) });
                                    dialogContent.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                                    TextBlock text = new TextBlock { Text = "Enter username", FontWeight = FontWeights.Light, FontSize = 22, Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)), HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
                                    dialogContent.Children.Add(text);
                                    zEdit username = new zEdit { Margin = new Thickness(24, 0, 24, 0), Height = 32, Text = ((string)item.Header) };
                                    Grid.SetRow(username, 1);
                                    dialogContent.Children.Add(username);
                                    StackPanel buttons = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
                                    zButton ok = new zButton { Text = "OK", Width = 96, Height = 24, Margin = new Thickness(8) };
                                    ok.click += (s4, e4) =>
                                    {
                                        dialog.DialogResult = true;
                                        dialog.Close();
                                    };
                                    zButton cancel = new zButton { Text = "Cancel", Width = 96, Height = 24, Margin = new Thickness(8) };
                                    cancel.click += (s4, e4) =>
                                    {
                                        dialog.DialogResult = false;
                                        dialog.Close();
                                    };
                                    buttons.Children.Add(ok);
                                    buttons.Children.Add(cancel);
                                    Grid.SetRow(buttons, 2);
                                    dialogContent.Children.Add(buttons);
                                    if (dialog.ShowDialog() == true && dialog.DialogResult == true)
                                    {
                                        if (username.Text == "")
                                        {
                                            MessageBox.Show("Can't create player without username", "SRMPEditor");
                                            return;
                                        }
                                        item.Header = username.Text;
                                    }
                                };
                                MenuItem delete = new MenuItem { Header = "Delete Player" };
                                delete.Click += (s3, e3) =>
                                {
                                    Window dialog = new Window { Width = 320, Height = 200, Title = "Delete Player", ResizeMode = ResizeMode.NoResize, Left = SystemParameters.PrimaryScreenWidth / 2 - 160, Top = SystemParameters.PrimaryScreenHeight / 2 - 100 };
                                    Grid dialogContent = new Grid { Background = new SolidColorBrush(Color.FromRgb(24, 24, 24)) };
                                    dialog.Content = dialogContent;
                                    dialogContent.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1.5, GridUnitType.Star) });
                                    dialogContent.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                                    TextBlock text = new TextBlock { Text = "Are you sure you want\nto delete this player's data?", FontWeight = FontWeights.Light, FontSize = 22, Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)), HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, TextAlignment = TextAlignment.Center };
                                    dialogContent.Children.Add(text);
                                    StackPanel buttons = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
                                    zButton yes = new zButton { Text = "Yes", Width = 96, Height = 24, Margin = new Thickness(8) };
                                    yes.click += (s4, e4) =>
                                    {
                                        dialog.DialogResult = true;
                                        dialog.Close();
                                    };
                                    zButton no = new zButton { Text = "No", Width = 96, Height = 24, Margin = new Thickness(8) };
                                    no.click += (s4, e4) =>
                                    {
                                        dialog.DialogResult = false;
                                        dialog.Close();
                                    };
                                    buttons.Children.Add(yes);
                                    buttons.Children.Add(no);
                                    Grid.SetRow(buttons, 1);
                                    dialogContent.Children.Add(buttons);
                                    if (dialog.ShowDialog() == true && dialog.DialogResult == true)
                                    {
                                        playersItem.Items.Remove(item);
                                        players.Remove(item.Header.ToString());
                                    }
                                };
                                playerMenu.Items.Add(rename);
                                playerMenu.Items.Add(delete);
                                playerMenu.PlacementTarget = item;
                                playerMenu.Background = new SolidColorBrush(Color.FromRgb(20, 20, 20));
                                playerMenu.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                                playerMenu.BorderThickness = new Thickness(0);
                                playerMenu.IsOpen = true;
                            };
                            playersItem.Items.Add(item);
                        }
                        else
                            MessageBox.Show("This player already exists", "SRMPEditor"); // change on "override this player data?"
                    }
                };
                MenuItem create = new MenuItem { Header = "Create Player" };
                create.Click += (s, e) =>
                {
                    Window dialog = new Window { Width = 320, Height = 200, Title = "Create a New Player", ResizeMode = ResizeMode.NoResize, Left = SystemParameters.PrimaryScreenWidth / 2 - 160, Top = SystemParameters.PrimaryScreenHeight / 2 - 100 };
                    Grid dialogContent = new Grid { Background = new SolidColorBrush(Color.FromRgb(24, 24, 24)) };
                    dialog.Content = dialogContent;
                    dialogContent.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.85, GridUnitType.Star) });
                    dialogContent.RowDefinitions.Add(new RowDefinition { Height = new GridLength(40, GridUnitType.Pixel) });
                    dialogContent.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                    TextBlock text = new TextBlock { Text = "Enter username", FontWeight = FontWeights.Light, FontSize = 22, Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)), HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
                    dialogContent.Children.Add(text);
                    zEdit username = new zEdit { Margin = new Thickness(24, 0, 24, 0), Height = 32, Text = "" };
                    Grid.SetRow(username, 1);
                    dialogContent.Children.Add(username);
                    StackPanel buttons = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
                    zButton ok = new zButton { Text = "OK", Width = 96, Height = 24, Margin = new Thickness(8) };
                    ok.click += (s2, e2) =>
                    {
                        dialog.DialogResult = true;
                        dialog.Close();
                    };
                    zButton cancel = new zButton { Text = "Cancel", Width = 96, Height = 24, Margin = new Thickness(8) };
                    cancel.click += (s2, e2) =>
                    {
                        dialog.DialogResult = false;
                        dialog.Close();
                    };
                    buttons.Children.Add(ok);
                    buttons.Children.Add(cancel);
                    Grid.SetRow(buttons, 2);
                    dialogContent.Children.Add(buttons);
                    if (dialog.ShowDialog() == true && dialog.DialogResult == true)
                    {
                        if (username.Text == "")
                        {
                            MessageBox.Show("Can't create player without username", "SRMPEditor");
                            return;
                        }
                        Player player = new Player();
                        players.Add(username.Text, player);

                        TreeViewItem item = new TreeViewItem { Header = username.Text, Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)) };
                        item.MouseDoubleClick += (s2, e2) =>
                        {
                            if (e2.RightButton != MouseButtonState.Released) return;
                            DoubleAnimation anim = new DoubleAnimation { To = 0, Duration = TimeSpan.FromMilliseconds(100) };
                            anim.Completed += async (s3, e3) =>
                            {
                                editorArea.Content = await createPlayerEditor(players[item.Header.ToString()], item.Header.ToString());
                                await Task.Delay(1);
                                DoubleAnimation animShow = new DoubleAnimation { To = 1, Duration = TimeSpan.FromMilliseconds(100) };
                                editorArea.BeginAnimation(OpacityProperty, animShow);
                            };
                            editorArea.BeginAnimation(OpacityProperty, anim);
                        };

                        item.MouseRightButtonUp += (s2, e2) =>
                        {
                            ((TreeViewItem)s2).IsSelected = true;
                            ContextMenu playerMenu = new ContextMenu();
                            MenuItem rename = new MenuItem { Header = "Rename Player" };
                            rename.Click += (s3, e3) =>
                            {
                                Window dialog1 = new Window { Width = 320, Height = 200, Title = "Rename Player", ResizeMode = ResizeMode.NoResize, Left = SystemParameters.PrimaryScreenWidth / 2 - 160, Top = SystemParameters.PrimaryScreenHeight / 2 - 100 };
                                Grid dialogContent1 = new Grid { Background = new SolidColorBrush(Color.FromRgb(24, 24, 24)) };
                                dialog1.Content = dialogContent1;
                                dialogContent1.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.85, GridUnitType.Star) });
                                dialogContent1.RowDefinitions.Add(new RowDefinition { Height = new GridLength(40, GridUnitType.Pixel) });
                                dialogContent1.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                                TextBlock text1 = new TextBlock { Text = "Enter username", FontWeight = FontWeights.Light, FontSize = 22, Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)), HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
                                dialogContent1.Children.Add(text1);
                                zEdit username1 = new zEdit { Margin = new Thickness(24, 0, 24, 0), Height = 32, Text = ((string)item.Header) };
                                Grid.SetRow(username1, 1);
                                dialogContent1.Children.Add(username1);
                                StackPanel buttons1 = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
                                zButton ok1 = new zButton { Text = "OK", Width = 96, Height = 24, Margin = new Thickness(8) };
                                ok1.click += (s4, e4) =>
                                {
                                    dialog1.DialogResult = true;
                                    dialog1.Close();
                                };
                                zButton cancel1 = new zButton { Text = "Cancel", Width = 96, Height = 24, Margin = new Thickness(8) };
                                cancel1.click += (s4, e4) =>
                                {
                                    dialog1.DialogResult = false;
                                    dialog1.Close();
                                };
                                buttons1.Children.Add(ok1);
                                buttons1.Children.Add(cancel1);
                                Grid.SetRow(buttons1, 2);
                                dialogContent1.Children.Add(buttons1);
                                if (dialog1.ShowDialog() == true && dialog1.DialogResult == true)
                                {
                                    if (username1.Text == "")
                                    {
                                        MessageBox.Show("Can't create player without username", "SRMPEditor");
                                        return;
                                    }
                                    item.Header = username1.Text;
                                }
                            };
                            MenuItem delete = new MenuItem { Header = "Delete Player" };
                            delete.Click += (s3, e3) =>
                            {
                                Window dialog1 = new Window { Width = 320, Height = 200, Title = "Delete Player", ResizeMode = ResizeMode.NoResize, Left = SystemParameters.PrimaryScreenWidth / 2 - 160, Top = SystemParameters.PrimaryScreenHeight / 2 - 100 };
                                Grid dialogContent1 = new Grid { Background = new SolidColorBrush(Color.FromRgb(24, 24, 24)) };
                                dialog1.Content = dialogContent1;
                                dialogContent1.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1.5, GridUnitType.Star) });
                                dialogContent1.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                                TextBlock text1 = new TextBlock { Text = "Are you sure you want\nto delete this player's data?", FontWeight = FontWeights.Light, FontSize = 22, Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)), HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, TextAlignment = TextAlignment.Center };
                                dialogContent1.Children.Add(text1);
                                StackPanel buttons1 = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
                                zButton yes = new zButton { Text = "Yes", Width = 96, Height = 24, Margin = new Thickness(8) };
                                yes.click += (s4, e4) =>
                                {
                                    dialog1.DialogResult = true;
                                    dialog1.Close();
                                };
                                zButton no = new zButton { Text = "No", Width = 96, Height = 24, Margin = new Thickness(8) };
                                no.click += (s4, e4) =>
                                {
                                    dialog1.DialogResult = false;
                                    dialog1.Close();
                                };
                                buttons1.Children.Add(yes);
                                buttons1.Children.Add(no);
                                Grid.SetRow(buttons1, 1);
                                dialogContent1.Children.Add(buttons1);
                                if (dialog1.ShowDialog() == true && dialog1.DialogResult == true)
                                {
                                    playersItem.Items.Remove(item);
                                    players.Remove(item.Header.ToString());
                                }
                            };
                            playerMenu.Items.Add(rename);
                            playerMenu.Items.Add(delete);
                            playerMenu.PlacementTarget = item;
                            playerMenu.Background = new SolidColorBrush(Color.FromRgb(20, 20, 20));
                            playerMenu.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                            playerMenu.BorderThickness = new Thickness(0);
                            playerMenu.IsOpen = true;
                        };
                        playersItem.Items.Add(item);
                    }
                };
                menu.Items.Add(add);
                menu.Items.Add(create);
                menu.PlacementTarget = playersItem;
                menu.Background = new SolidColorBrush(Color.FromRgb(20, 20, 20));
                menu.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                menu.BorderThickness = new Thickness(0);
                menu.IsOpen = true;
            };
            dockPanel.Children.Add(worldButton);
            dockPanel.Children.Add(playersButton);

            return grid;
        }

        bool editorLoaded;
        bool[] filtersMem = { true, false, true, true, true, true };

        public async Task<Grid> createPlayerEditor(Player player, string playerName)
        {
            editorLoaded = false;
            zSwitch enableWaterSlot = null;
            await Task.Delay(16);
            Grid grid = new Grid { Margin = new Thickness(8, 0, 0, 0) };
            StackPanel panel = new StackPanel { HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top };
            TextBlock title = new TextBlock { Text = playerName, FontSize = 36, FontWeight = FontWeights.Light, Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)), Margin = new Thickness(8, 16, 8, 8), HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top };
            panel.Children.Add(title);

            StackPanel health = new StackPanel { Orientation = Orientation.Vertical, Height = 60 };
            TextBlock healthText = new TextBlock { Text = "Health: ", FontSize = 14, FontWeight = FontWeights.Light, Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)), Margin = new Thickness(8, 8, 8, 0), HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Center };
            zEdit healthEdit = new zEdit { Text = Math.Round(player.Model.currHealth).ToString(), Width = 192, Height = 36, Padding = new Thickness(8, 8, 8, 4), HorizontalAlignment = HorizontalAlignment.Left };
            healthEdit.TextChanged += (s, e) =>
            {
                string before = ((TextBox)s).Text;
                ((TextBox)s).Text = Regex.Replace(((TextBox)s).Text, @"[^\d]", "");
                if (before != ((TextBox)s).Text) ((TextBox)s).CaretIndex = ((TextBox)s).Text.Length;
                float.TryParse(((TextBox)s).Text, out player.Model.currHealth);
                if (player.Model.currHealth < 0)
                {
                    if (((TextBox)s).Text != "")
                        ((TextBox)s).Text = "0";
                    player.Model.currHealth = 0;
                }
            };
            health.Children.Add(healthText);
            health.Children.Add(healthEdit);
            panel.Children.Add(health);


            StackPanel energy = new StackPanel { Orientation = Orientation.Vertical, Height = 60 };
            TextBlock energyText = new TextBlock { Text = "Energy: ", FontSize = 14, FontWeight = FontWeights.Light, Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)), Margin = new Thickness(8, 8, 8, 0), HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Center };
            zEdit energyEdit = new zEdit { Text = Math.Round(player.Model.currEnergy).ToString(), Width = 192, Height = 36, Padding = new Thickness(8, 8, 8, 4), HorizontalAlignment = HorizontalAlignment.Left };
            energyEdit.TextChanged += (s, e) =>
            {
                string before = ((TextBox)s).Text;
                ((TextBox)s).Text = Regex.Replace(((TextBox)s).Text, @"[^\d]", "");
                if (before != ((TextBox)s).Text) ((TextBox)s).CaretIndex = ((TextBox)s).Text.Length;
                float.TryParse(((TextBox)s).Text, out player.Model.currEnergy);
                if (player.Model.currEnergy < 0)
                {
                    if (((TextBox)s).Text != "")
                        ((TextBox)s).Text = "0";
                    player.Model.currEnergy = 0;
                }
            };
            energy.Children.Add(energyText);
            energy.Children.Add(energyEdit);
            panel.Children.Add(energy);

            StackPanel currency = new StackPanel { Orientation = Orientation.Vertical, Height = 60 };
            TextBlock currencyText = new TextBlock { Text = "Currency: ", FontSize = 14, FontWeight = FontWeights.Light, Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)), Margin = new Thickness(8, 8, 8, 0), HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Center };
            zEdit currencyEdit = new zEdit { Text = player.Model.currency.ToString(), Width = 192, Height = 36, Padding = new Thickness(8, 8, 8, 4), HorizontalAlignment = HorizontalAlignment.Left };
            currencyEdit.TextChanged += (s, e) =>
            {
                string before = ((TextBox)s).Text;
                ((TextBox)s).Text = Regex.Replace(((TextBox)s).Text, @"[^\d]", "");
                if (before != ((TextBox)s).Text) ((TextBox)s).CaretIndex = ((TextBox)s).Text.Length;
                int.TryParse(((TextBox)s).Text, out player.Model.currency);
                if (player.Model.currency < 0)
                {
                    if (((TextBox)s).Text != "")
                        ((TextBox)s).Text = "0";
                    player.Model.currency = 0;
                }
            };
            currency.Children.Add(currencyText);
            currency.Children.Add(currencyEdit);
            panel.Children.Add(currency);

            StackPanel slotsPanel = new StackPanel { HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top };
            TextBlock itemSlotsText = new TextBlock { Text = "Item slots", FontSize = 22, FontWeight = FontWeights.Light, Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)), Margin = new Thickness(8), Height = 24, Width = 180, HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top };
            slotsPanel.Children.Add(itemSlotsText);
            StackPanel filterPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, -2, 0, 4) };
            TextBlock filter = new TextBlock { Text = "Filters: ", Margin = new Thickness(8, 0, 2, 0), FontSize = 16, FontWeight = FontWeights.Light, Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)) };
            StackPanel switches = new StackPanel { Orientation = Orientation.Horizontal, VerticalAlignment = VerticalAlignment.Bottom };
            List<ComboBox> slots = new List<ComboBox>();
            string[] itemTypes = Enum.GetNames(typeof(itemEntry.itemType));

            for (int j = 0; j < 6; j++) {
                int currentJ = j;
                zSwitch sw = new zSwitch { Text = itemTypes[j], Margin = new Thickness(0, 0, 2, 0), isChecked = filtersMem[j] };
                sw.click += (s, e) =>
                {
                    foreach(ComboBox itemSlot in slots) {
                            string prev = itemSlot.Text;
                            itemSlot.Items.Clear();
                            itemSlot.Items.Add("[Empty]");
                        for (int k = 0; k < switches.Children.Count; k++) {
                            foreach (KeyValuePair<string, itemEntry> entry in items)
                                if (!entry.Value.name.Contains("["))
                                        if (((zSwitch)switches.Children[k]).isChecked && entry.Value.type.ToString() == itemTypes[k])
                                            itemSlot.Items.Add(entry.Value.name);
                        }
                        if (!itemSlot.Items.Contains(prev))
//                            if(itemSlot.Items.Count != 0)
                                itemSlot.Items.Insert(1, prev);
//                            else
//                                itemSlot.Items.Add(prev);

                        itemSlot.Text = prev;
                    }
                    filtersMem[currentJ] = !filtersMem[currentJ];
                };
                switches.Children.Add(sw);
            }

            filterPanel.Children.Add(filter);
            filterPanel.Children.Add(switches);
            slotsPanel.Children.Add(filterPanel);

            for (int i = 0; i < 5; i++) {
                int currentI = i;
                Grid slot = new Grid { Margin = new Thickness(8) };
                slot.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(160) });
                slot.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(8) });
                slot.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(64) });
                slot.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(8) });
                slot.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                ComboBox itemSlot = new ComboBox { Height = 24 };
                bool isEmpty = false;
                
                zEdit itemSlotCount = new zEdit { Height = 24, Text = "" };
                if (player.Model.ammoDict[0].slots[i] != null)
                    if (items.ContainsKey(player.Model.ammoDict[0].slots[i].id.ToString()))
                        if (items[player.Model.ammoDict[0].slots[i].id.ToString()].name.Contains("["))
                        {
                            itemSlot.Items.Add("[" + items[player.Model.ammoDict[0].slots[i].id.ToString()].name.Substring(1, items[player.Model.ammoDict[0].slots[i].id.ToString()].name.Length - 2) + "; ID: " + player.Model.ammoDict[0].slots[i].id.ToString() + "]");
                            itemSlot.Text = "[" + items[player.Model.ammoDict[0].slots[i].id.ToString()].name.Substring(1, items[player.Model.ammoDict[0].slots[i].id.ToString()].name.Length - 2) + "; ID: " + player.Model.ammoDict[0].slots[i].id.ToString() + "]";
                        }
                        else
                            itemSlot.Text = items[player.Model.ammoDict[0].slots[i].id.ToString()].name.ToString();
                    else
                    {
                        itemSlot.Items.Add("[?; ID: " + player.Model.ammoDict[0].slots[i].id.ToString() + "]");
                        itemSlot.Text = "[?; ID: " + player.Model.ammoDict[0].slots[i].id.ToString() + "]";
                    }
                else
                    isEmpty = true;
                itemSlot.Items.Insert(0, "[Empty]");
                if(isEmpty)
                    itemSlot.Text = "[Empty]";
                if (player.Model.ammoDict[0].slots[i] != null)
                    itemSlotCount.Text = player.Model.ammoDict[0].slots[i].count.ToString();
                itemSlotCount.TextChanged += (s, e) =>
                {
                    if(itemSlot.Text == "[Empty]") {
                        ((TextBox)s).Text = "";
                        return;
                    }
                    string before = ((TextBox)s).Text;
                    ((TextBox)s).Text = Regex.Replace(((TextBox)s).Text, @"[^\d]", "");
                    if (before != ((TextBox)s).Text) ((TextBox)s).CaretIndex = ((TextBox)s).Text.Length;
                    if (player.Model.ammoDict[0].slots[currentI] != null) {
                        int.TryParse(((TextBox)s).Text, out player.Model.ammoDict[0].slots[currentI].count);
                        if(player.Model.ammoDict[0].slots[currentI].count > 2097152) {
                            ((TextBox)s).Text = "2097152";
                            ((TextBox)s).CaretIndex = ((TextBox)s).Text.Length;
                        } else if (((TextBox)s).Text == "" || ((TextBox)s).Text == "0")
                            player.Model.ammoDict[0].slots[currentI].count = 1;
                    } else
                            ((TextBox)s).Text = "";
                };
                Grid.SetColumn(itemSlotCount, 2);

                itemSlot.SelectionChanged += (s, e) => {
                    if (e.AddedItems.Count == 0 || ((ComboBox)s).Items.Count == 0 || ((ComboBox)s).Items.Count == 1 || !editorLoaded) return;

                    if (((string)e.AddedItems[0]) == "[Empty]") {
                        player.Model.ammoDict[0].slots[currentI] = null;
                        itemSlotCount.Text = "";
                        return;
                    }


                    if (itemsByName.ContainsKey(((string)e.AddedItems[0])))
                        if (player.Model.ammoDict[0].slots[currentI] == null) {
                            Slot newSlot = new Slot { count = 1, emotions = new Dictionary<byte, float>() };
                            ushort.TryParse(itemsByName[((string)e.AddedItems[0])].id.ToString(), out newSlot.id);
                            player.Model.ammoDict[0].slots[currentI] = newSlot;
                            itemSlotCount.Text = "1";
                        } else {
                            ushort.TryParse(itemsByName[((string)e.AddedItems[0])].id.ToString(), out player.Model.ammoDict[0].slots[currentI].id);
                            player.Model.ammoDict[0].slots[currentI].emotions = new Dictionary<byte, float>();
                        }
                };

                slot.Children.Add(itemSlot);
                slot.Children.Add(itemSlotCount);
                if (i == 4) {
                    bool defaultWS = false;
                    if (player.Model.ammoDict[0].usableSlots == 5) defaultWS = true;
                    enableWaterSlot = new zSwitch { Text = "Enable Liquid Slot", isChecked = defaultWS };
                    enableWaterSlot.click += (s, e) =>
                    {
                        if (((zSwitch)s).isChecked)
                            player.Model.ammoDict[0].usableSlots = 5;
                        else
                            player.Model.ammoDict[0].usableSlots = 4;
                    };
                    Grid.SetColumn(enableWaterSlot, 4);
                    slot.Children.Add(enableWaterSlot);
                }

                slots.Add(itemSlot);
                slotsPanel.Children.Add(slot);
            }

            foreach (ComboBox itemSlot in slots)
            {
                string prev = itemSlot.Text;
                itemSlot.Items.Clear();
                itemSlot.Items.Add("[Empty]");
                for (int k = 0; k < switches.Children.Count; k++)
                {
                    foreach (KeyValuePair<string, itemEntry> entry in items)
                        if (!entry.Value.name.Contains("["))
                            if (((zSwitch)switches.Children[k]).isChecked && entry.Value.type.ToString() == itemTypes[k])
                                itemSlot.Items.Add(entry.Value.name);
                }
                for (int i = 0; i < slots.Count; i++)
                {
                    if (player.Model.ammoDict[0].slots[i] != null)
                        if (items.ContainsKey(player.Model.ammoDict[0].slots[i].id.ToString()))
                            if (items[player.Model.ammoDict[0].slots[i].id.ToString()].name.Contains("["))
                            {
                                itemSlot.Items.Add("[" + items[player.Model.ammoDict[0].slots[i].id.ToString()].name.Substring(1, items[player.Model.ammoDict[0].slots[i].id.ToString()].name.Length - 2) + "; ID: " + player.Model.ammoDict[0].slots[i].id.ToString() + "]");
                                itemSlot.Text = "[" + items[player.Model.ammoDict[0].slots[i].id.ToString()].name.Substring(1, items[player.Model.ammoDict[0].slots[i].id.ToString()].name.Length - 2) + "; ID: " + player.Model.ammoDict[0].slots[i].id.ToString() + "]";
                            }
                            else
                                itemSlot.Text = items[player.Model.ammoDict[0].slots[i].id.ToString()].name.ToString();
                        else
                        {
                            itemSlot.Items.Add("[?; ID: " + player.Model.ammoDict[0].slots[i].id.ToString() + "]");
                            itemSlot.Text = "[?; ID: " + player.Model.ammoDict[0].slots[i].id.ToString() + "]";
                        }
                    else
                        itemSlot.Text = "[Empty]";
                }
                if (!itemSlot.Items.Contains(prev)) itemSlot.Items.Insert(1, prev);
                itemSlot.Text = prev;
            }

            panel.Children.Add(slotsPanel);

            if (!world.SharedUpgrades)
            {
                Border ubg = new Border { Background = new SolidColorBrush(Color.FromRgb(20, 20, 20)), CornerRadius = new CornerRadius(8), Margin = new Thickness(8, 8, 8, 4), HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top, Padding = new Thickness(0, 0, 0, 8), Width = 512, SnapsToDevicePixels = true };
                WrapPanel upgrades = new WrapPanel { Orientation = Orientation.Horizontal };
                ubg.Child = upgrades;
                upgradeElementsList = new Dictionary<string, Border>();
                addUpgrade = new ComboBox { Height = 24, Margin = new Thickness(8, 8, 0, 0), Width = 18 };
                addUpgrade.DropDownOpened += (s, e) =>
                {
                    if (((ComboBox)s).Items.Count == 0)
                        addUpgrade.IsDropDownOpen = false;
                };
                addUpgrade.SelectionChanged += (s, e) =>
                {
                    if (e.AddedItems.Count == 0 || ((ComboBox)s).Items.Count == 0 || !editorLoaded) 
                        return;
                    ((ComboBox)s).Items.Remove(e.AddedItems[0]);
                    upgradeElementsList[(string)e.AddedItems[0]].Visibility = Visibility.Visible;
                    player.Model.upgrades.Add(Convert.ToByte((int)Enum.Parse(typeof(Upgrade), ((string)e.AddedItems[0]))));
                    addUpgrade.Items.Clear();
                    foreach (Upgrade u1 in Enum.GetValues(typeof(Upgrade)))
                    {
                        if (!player.Model.upgrades.Contains(Convert.ToByte((int)u1)))
                            addUpgrade.Items.Add(u1.ToString());
                    }
                    if (((string)e.AddedItems[0]).Equals(Upgrade.LIQUID_SLOT.ToString()) && enableWaterSlot != null)
                    {
                        enableWaterSlot.setChecked(true, false);
                        player.Model.ammoDict[0].usableSlots = 5;
                    }
                };
                foreach (Upgrade u in Enum.GetValues(typeof(Upgrade)))
                {
                    Border bg = new Border { Background = new SolidColorBrush(Color.FromRgb(24, 24, 24)), CornerRadius = new CornerRadius(8), Margin = new Thickness(8, 8, 0, 0), Height = 24, Visibility = Visibility.Collapsed, VerticalAlignment = VerticalAlignment.Top, HorizontalAlignment = HorizontalAlignment.Left, SnapsToDevicePixels = true };
                    Grid g = new Grid();
                    bg.Child = g;
                    g.ColumnDefinitions.Add(new ColumnDefinition());
                    g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(24) });
                    TextBlock text = new TextBlock { Text = u.ToString(), Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)), HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(8, 0, 4, 0) };
                    if (!player.Model.upgrades.Contains(Convert.ToByte((int)u)))
                        addUpgrade.Items.Add(u.ToString());
                    else
                        bg.Visibility = Visibility.Visible;
                    zButton remove = new zButton { Text = "", Width = 16, Height = 16, Path = xShape, IconWidth = new GridLength(16), HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, Corner = new CornerRadius(8), colorClicked = Color.FromRgb(20, 20, 20), color = Color.FromRgb(24, 24, 24), colorHover = Color.FromRgb(36, 36, 36) };
                    remove.click += (s, e) =>
                    {
                        bg.Visibility = Visibility.Collapsed;
                        player.Model.upgrades.Remove(Convert.ToByte((int)u));
                        addUpgrade.Items.Clear();
                        foreach (Upgrade u1 in Enum.GetValues(typeof(Upgrade)))
                        {
                            if (!player.Model.upgrades.Contains(Convert.ToByte((int)u1)))
                                addUpgrade.Items.Add(u1.ToString());
                        }
                        if (u.ToString().Equals(Upgrade.LIQUID_SLOT.ToString()) && enableWaterSlot != null)
                        {
                            enableWaterSlot.setChecked(false, false);
                            player.Model.ammoDict[0].usableSlots = 4;
                        }
                    };
                    Grid.SetColumn(remove, 1);
                    g.Children.Add(text);
                    g.Children.Add(remove);
                    upgradeElementsList.Add(u.ToString(), bg);
                    upgrades.Children.Add(bg);
                }
                upgrades.Children.Add(addUpgrade);

                panel.Children.Add(ubg);
            }
            grid.Children.Add(panel);
            editorLoaded = true;
            return grid;
        }
        public ComboBox addUpgrade;
        public Dictionary<string, Border> upgradeElementsList = new Dictionary<string, Border>();
        public async Task<Grid> createWorldEditor()
        {
            editorLoaded = false;
            await Task.Delay(16);
            Grid grid = new Grid { Margin = new Thickness(8, 0, 0, 0) };
            StackPanel panel = new StackPanel { HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top };
            TextBlock title = new TextBlock { Text = "World settings", FontSize = 36, FontWeight = FontWeights.Light, Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)), Margin = new Thickness(8, 16, 8, 8), HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top };
            panel.Children.Add(title);
            zEdit currency = new zEdit { Width = 192, Height = 24, Text = world.TotalCurrency.ToString(), Margin = new Thickness(8, 8, 8, 4), HorizontalAlignment = HorizontalAlignment.Left };
            zSwitch isSharedCurrency = new zSwitch { Text = "Shared Currency", Margin = new Thickness(8, 8, 8, 4), isChecked = world.SharedCurrency };
            if (world.SharedCurrency)
                currency.Visibility = Visibility.Visible;
            else
                currency.Visibility = Visibility.Collapsed;
            isSharedCurrency.click += (s, e) =>
            {
                world.SharedCurrency = ((zSwitch)s).isChecked;
                if (world.SharedCurrency)
                    currency.Visibility = Visibility.Visible;
                else
                    currency.Visibility = Visibility.Collapsed;
            };
            currency.TextChanged += (s, e) =>
            {
                string before = ((TextBox)s).Text;
                ((TextBox)s).Text = Regex.Replace(((TextBox)s).Text, @"[^\d]", "");
                if (before != ((TextBox)s).Text) ((TextBox)s).CaretIndex = ((TextBox)s).Text.Length;
                    int.TryParse(((TextBox)s).Text, out world.TotalCurrency);
                if (world.TotalCurrency < 0)
                {
                    if(((TextBox)s).Text != "")
                        ((TextBox)s).Text = "0";
                    world.TotalCurrency = 0;
                }
            };
            panel.Children.Add(isSharedCurrency);
            panel.Children.Add(currency);
            Border ubg = new Border { Background = new SolidColorBrush(Color.FromRgb(20, 20, 20)), CornerRadius = new CornerRadius(8), Margin = new Thickness(8, 8, 8, 4), HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top, Padding = new Thickness(0, 0, 0, 8), Width = 512, SnapsToDevicePixels = true };
            WrapPanel upgrades = new WrapPanel { Orientation = Orientation.Horizontal };
            ubg.Child = upgrades;
            upgradeElementsList = new Dictionary<string, Border>();
            addUpgrade = new ComboBox { Height = 24, Margin = new Thickness(8, 8, 0, 0), Width = 18 };
            addUpgrade.DropDownOpened += (s, e) =>
            {
                if (((ComboBox)s).Items.Count == 0)
                    addUpgrade.IsDropDownOpen = false;
            };
            addUpgrade.SelectionChanged += (s, e) =>
            {
                if (e.AddedItems.Count == 0 || ((ComboBox)s).Items.Count == 0 || !editorLoaded)
                    return;
                ((ComboBox)s).Items.Remove(e.AddedItems[0]);
                upgradeElementsList[(string)e.AddedItems[0]].Visibility = Visibility.Visible;
                world.AllUpgrades.Add(Convert.ToByte((int)Enum.Parse(typeof(Upgrade), ((string)e.AddedItems[0]))));
                addUpgrade.Items.Clear();
                foreach (Upgrade u1 in Enum.GetValues(typeof(Upgrade)))
                {
                    if (!world.AllUpgrades.Contains(Convert.ToByte((int)u1)))
                        addUpgrade.Items.Add(u1.ToString());
                }

                if (((string)e.AddedItems[0]).Equals(Upgrade.LIQUID_SLOT.ToString()))
                    foreach( Player player in players.Values )
                        player.Model.ammoDict[0].usableSlots = 5;
            };
            foreach (Upgrade u in Enum.GetValues(typeof(Upgrade)))
            {
                Border bg = new Border { Background = new SolidColorBrush(Color.FromRgb(24, 24, 24)), CornerRadius = new CornerRadius(8), Margin = new Thickness(8, 8, 0, 0), Height = 24, Visibility = Visibility.Collapsed, VerticalAlignment = VerticalAlignment.Top, HorizontalAlignment = HorizontalAlignment.Left, SnapsToDevicePixels = true };
                Grid g = new Grid();
                bg.Child = g;
                g.ColumnDefinitions.Add(new ColumnDefinition());
                g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(24) });
                TextBlock text = new TextBlock { Text = u.ToString(), Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)), HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(8, 0, 4, 0) };
                if (!world.AllUpgrades.Contains(Convert.ToByte((int)u)))
                    addUpgrade.Items.Add(u.ToString());
                else
                    bg.Visibility = Visibility.Visible;
                zButton remove = new zButton { Text = "", Width = 16, Height = 16, Path = xShape, IconWidth = new GridLength(16), HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, Corner = new CornerRadius(8), colorClicked = Color.FromRgb(20, 20, 20), color = Color.FromRgb(24, 24, 24), colorHover = Color.FromRgb(36, 36, 36) };
                remove.click += (s, e) =>
                {
                    bg.Visibility = Visibility.Collapsed;
                    world.AllUpgrades.Remove(Convert.ToByte((int)u));
                    addUpgrade.Items.Clear();
                    foreach (Upgrade u1 in Enum.GetValues(typeof(Upgrade)))
                    {
                        if (!world.AllUpgrades.Contains(Convert.ToByte((int)u1)))
                            addUpgrade.Items.Add(u1.ToString());
                    }
                    if (u.ToString().Equals(Upgrade.LIQUID_SLOT.ToString()))
                        foreach (Player player in players.Values)
                            player.Model.ammoDict[0].usableSlots = 4;
                };
                Grid.SetColumn(remove, 1);
                g.Children.Add(text);
                g.Children.Add(remove);
                upgradeElementsList.Add(u.ToString(), bg);
                upgrades.Children.Add(bg);
            }
            upgrades.Children.Add(addUpgrade);

            if (world.SharedUpgrades)
                ubg.Visibility = Visibility.Visible;
            else
                ubg.Visibility = Visibility.Collapsed;
            zSwitch isSharedUpgrades = new zSwitch { Text = "Shared Upgrades", Margin = new Thickness(8, 8, 8, 4), isChecked = world.SharedUpgrades };
            isSharedUpgrades.click += (s, e) =>
            {
                world.SharedUpgrades = ((zSwitch)s).isChecked;
                if(world.SharedUpgrades)
                    ubg.Visibility = Visibility.Visible;
                else
                    ubg.Visibility = Visibility.Collapsed;
            };
            panel.Children.Add(isSharedUpgrades);
            panel.Children.Add(ubg);
            grid.Children.Add(panel);
            editorLoaded = true;
            return grid;
        }
    }
}
