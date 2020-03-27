using System;
using System.Windows.Forms;
using GTA;
using NativeUI;
using System.Xml.Linq;

public class MainScript : Script
{

    private MenuPool myMenuPool;
    private UIMenu myMenu;
    private XDocument xdoc;

    public MainScript()
    {
        Tick += OnTick;
        KeyDown += OnKeyDown;
        KeyUp += OnKeyUp;

        Interval = 10;
        myMenuPool = new MenuPool();
        myMenu = new UIMenu("Park Mod", "Par Tony Landschoot");
        myMenuPool.Add(myMenu);
        xdoc = XDocument.Load(@"C:\GTA_SaveVehicles\Vehicle.xml");
        foreach (var result in xdoc.Root.Elements("vehicle"))
        {
            myMenu.AddItem(new UIMenuItem(result.Element("text").Value));
        }
        myMenu.OnItemSelect += ItemSelectHandler;
        myMenu.RefreshIndex();

    }

    public void ItemSelectHandler(UIMenu sender, UIMenuItem selectedItem, int index)
    {
        bool flag = false;
        foreach (var result in xdoc.Root.Elements("vehicle"))
        {
            if(result.Element("text").Value == selectedItem.Text)
            {
                GTA.UI.Notification.Show(selectedItem.Text + " spawned");
                World.CreateVehicle("" + result.Element("hash").Value, Game.Player.Character.Position);
                flag = true;
                break;
            } 
        }
        if (!flag)
        {
            GTA.UI.Notification.Show("Vehicle not found");
        }
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
    }

    public void OnKeyUp(object sender, EventArgs e)
    {

    }
}