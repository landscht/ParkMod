using System;
using System.Windows.Forms;
using GTA;
using NativeUI;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.IO;
using ParkMod.Properties;

public class MainScript : Script
{

    private MenuPool myMenuPool;
    private UIMenu myMenu;
    private XDocument xdoc;
    private List<Vehicle> vehicles;
    private List<VehicleDTO> vehicleDTOs;

    public MainScript()
    {
        Tick += OnTick;
        KeyDown += OnKeyDown;
        KeyUp += OnKeyUp;

        // Menu
        myMenuPool = new MenuPool();
        myMenu = new UIMenu("Park Mod", "Par Tony Landschoot (v.0.2)");
        myMenuPool.Add(myMenu);
        var infoItem = new UIMenuItem("Sauvegarder le véhicule", "Permet d'ajouter le véhicule courant à votre park. Sa position, sa configuration ainsi que son etat sera sauvegardé");
        myMenu.AddItem(infoItem);


        // load vehicles into the List and actualize menu
        vehicles = new List<Vehicle>();
        vehicleDTOs = new List<VehicleDTO>();
        xdoc = XDocument.Load(@"C:\GTA_SaveVehicles\Vehicle.xml");
        foreach (var result in xdoc.Root.Elements("VehicleDTO"))
        {
            myMenu.AddItem(new UIMenuItem(result.Element("text").Value, "Voir les options pour " + result.Element("text")));
            Vehicle vehicle = World.CreateVehicle(result.Element("hash").Value,
                new GTA.Math.Vector3(float.Parse(result.Element("xPos").Value.Replace(".",",")),
                                        float.Parse(result.Element("yPos").Value.Replace(".", ",")),
                                        float.Parse(result.Element("zPos").Value.Replace(".",","))));
            vehicle.IsPersistent = true;
            vehicles.Add(vehicle);
        }

        // save vehicle
        myMenu.OnItemSelect += (sender, item, inex) =>
        {
            if (item == infoItem)
            {
                if (vehicles.Contains(Game.Player.LastVehicle))
                {
                    GTA.UI.Notification.Show("Véhicule déja sauvegardé");
                }
                else
                {
                    vehicles.Add(Game.Player.LastVehicle);
                    Game.Player.LastVehicle.IsPersistent = true;
                    myMenu.AddItem(new UIMenuItem(Game.Player.LastVehicle.DisplayName, "Voir les options pour " + Game.Player.LastVehicle.DisplayName));
                    SaveVehicles();
                    GTA.UI.Notification.Show(Game.Player.LastVehicle.DisplayName + " est sauvegardé");
                }

            }
        };
        myMenu.RefreshIndex();
    }

    public void SaveVehicles()
    {
        vehicleDTOs.Clear();
        foreach(Vehicle v in vehicles)
        {
            vehicleDTOs.Add(new VehicleDTO
            {
                hash = v.DisplayName,
                text = v.DisplayName,
                xPos = v.Position.X,
                yPos = v.Position.Y,
                zPos = v.Position.Z,
            });
        }
        
        XmlSerializer xs = new XmlSerializer(typeof(List<VehicleDTO>));
        using (StreamWriter wr = new StreamWriter(@"C:\GTA_SaveVehicles\Vehicle.xml"))
        {
            xs.Serialize(wr, vehicleDTOs);
        }
        GTA.UI.Notification.Show("Positions sauvegardées");
    }

    public void OnTick(object sender, EventArgs e)
    {
        myMenuPool.ProcessMenus();

    }

    public void OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.F6) // Our menu on/off switch
        {
            myMenu.Visible = !myMenu.Visible;
        }

        if(e.KeyCode == Keys.F)
        {
            if(vehicles.Contains(Game.Player.LastVehicle) && Game.Player.Character.IsInVehicle())
            {
                SaveVehicles();
            }
        }
    }

    public void OnKeyUp(object sender, EventArgs e)
    {

    }
}