using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Resources;

namespace MTGApro
{
    /// <summary>
    /// Логика взаимодействия для Window4.xaml
    /// </summary>
    public partial class Window4 : Window
    {

        private Window5 win5 = new Window5();

        public static bool windowhidden=false;
        public static string deckplaying;
        public static string matchplayingwe;
        public static string matchplayingthey;
        public static int PackOpening =-1;
        public static int PickMacking =-1;
        public static string downloading;
        public static bool decksrendered=false;

        public static bool wasshown = false;

        public static string mode = @"me";
        public static BackgroundWorker Timer = new BackgroundWorker();
        public static BackgroundWorker Cardloader = new BackgroundWorker();

        public static bool rendering = false;

        public static int cidover = 0;
        public static Dictionary<string, int> topmargin = new Dictionary<string, int>() { { @"me", 20 }, { @"opponent", 20 }, { @"draft", 20 }, { @"decks", 20 } };
        public static Point location = new Point();
        public static Dictionary<int, Card> cdb = new Dictionary<int, Card>();
        public static Dictionary<int, int> cdb_mtga_id = new Dictionary<int, int>();
        public static Dictionary<int, int> collection = new Dictionary<int, int>();

        public static Dictionary<int,int> oldUdecklive = new Dictionary<int, int>();
        public static Dictionary<int, int> oldEdecklive = new Dictionary<int, int>();

        public static Dictionary<string, Dictionary<int, TextBlock>> cnums = new Dictionary<string, Dictionary<int, TextBlock>> { { "me", new Dictionary<int, TextBlock>() }, { "opponent", new Dictionary<int, TextBlock>() }, { "draft", new Dictionary<int, TextBlock>() }, { "decks", new Dictionary<int, TextBlock>() } };
        public static Dictionary<string, Dictionary<int, Border>> borders = new Dictionary<string, Dictionary<int, Border>> { {"me", new Dictionary<int, Border>()}, { "opponent", new Dictionary<int, Border>()}, { "draft", new Dictionary<int, Border>() }, { "decks", new Dictionary<int, Border>() } };
        public static Dictionary<string, string> colors = new Dictionary<string, string>{{ "White", "bfb66b" }, { "Blue", "4693b9" }, { "Black", "4e4442" }, { "Red", "c35d3a" }, { "Green", "448359" }, { "Multicolor", "d6be73" }, { "Colorless", "7a7777" } };
        public static Dictionary<string, string> colors_light = new Dictionary<string, string> { { "White", "bfb66b" }, { "Blue", "8fbdd3" }, { "Black", "726360" }, { "Red", "da8b70" }, { "Green", "92d1a7" }, { "Multicolor", "d6be73" }, { "Colorless", "7a7777" } };
        public static Dictionary<string, string> colors_dark = new Dictionary<string, string> { { "White", "7f7947" }, { "Blue", "28556b" }, { "Black", "4e4442" }, { "Red", "7c3b25" }, { "Green", "2d563a" }, { "Multicolor", "6c5611" }, { "Colorless", "7a7777" } };
        public static Dictionary<string, string> colors_font = new Dictionary<string, string> { { "White", "BDffffff" }, { "Blue", "BDffffff" }, { "Black", "BDffffff" }, { "Red", "BDffffff" }, { "Green", "BDffffff" }, { "Multicolor", "BDffffff" }, { "Colorless", "BDffffff" } };
        public static Dictionary<string, string> icons_mana = new Dictionary<string, string> { { "White", "\ue600" }, { "Blue", "\ue601" }, { "Black", "\ue602" }, { "Red", "\ue603" }, { "Green", "\ue604" }, { "1", "\ue606" }, { "2", "\ue607" }, { "3", "\ue608" }, { "4", "\ue609" }, { "5", "\ue60a" }, { "6", "\ue60b" }, { "7", "\ue60c" }, { "8", "\ue60d" }, { "9", "\ue60e" }, { "10", "\ue60f" }, { "11", "\ue610" }, { "12", "\ue611" }, { "13", "\ue612" }, { "14", "\ue613" }, { "15", "\ue614" }, { "16", "\ue62a" }, { "17", "\ue62b" }, { "18", "\ue62c" }, { "19", "\ue62d" }, { "20", "\ue62e" }, { "X", "\ue615" } };


        public class Card
        {
            public int Id { get; set; }
            public int Doublelink { get; set; }
            public string Cardid { get; set; }
            public int Multiverseid { get; set; }
            public string Name { get; set; }
            public string Slug { get; set; }
            public string Kw { get; set; }
            public string Flavor { get; set; }
            public int Power { get; set; }
            public int Toughness { get; set; }
            public int Expansion { get; set; }
            public int Rarity { get; set; }
            public string Mana { get; set; }
            public int Convmana { get; set; }
            public int Loyalty { get; set; }
            public int Type { get; set; }
            public int Subtype { get; set; }
            public string Txttype { get; set; }
            public string Pict { get; set; }
            public string Date_in { get; set; }
            public string Colorindicator { get; set; }
            public int Mtga_id { get; set; }
            public int Is_collectible { get; set; }
            public int Reprint { get; set; }
            public int Supercls { get; set; }
            public int Draftrate { get; set; }
            public int Drafteval { get; set; }
            public int Is_land { get; set; }
            public string Colorarr { get; set; }
            public int Currentstandard { get; set; }
            public string Art { get; set; }
        }

        public class Battle
        {
            public Dictionary<int, int> Udecklive { get; set; }
            public Dictionary<int, int> Edeck { get; set; }
            public Dictionary<int, int> Deckstruct { get; set; }
            public string Udeck_fp { get; set; }
            public string Edeckname { get; set; }
            public string Humanname { get; set; }

            public Battle(Dictionary<int, int> deckstruct, string udeck_fp, string humanname = @"", string edeckname=@"")
            {
                Udecklive = new Dictionary<int, int>();
                Edeck = new Dictionary<int, int>();
                Edeckname = edeckname;
                Deckstruct = deckstruct;
                Udeck_fp = udeck_fp;
                Humanname = humanname;
            }
        }

        public class Deck
        {
            public int Id { get; set; }
            public string Classstruct { get; set; }
            public string Humanname { get; set; }
            public float wlnratio { get; set; }
            public float perswinratio { get; set; }
        }

        //Load overlay settings
        public void GetOverlayData()
        {
            try
            {
                RegistryKey RkApp = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\\MTGAProtracker");
                if (RkApp != null)
                {

                    try
                    {
                        string op = RkApp.GetValue("op").ToString();
                        string size = RkApp.GetValue("size").ToString();
                        string posX = RkApp.GetValue("posX").ToString();
                        string posY = RkApp.GetValue("posY").ToString();
                        rendering = true;
                        if (!String.IsNullOrWhiteSpace(op)) Opacity=Convert.ToDouble(op);
                        if (!String.IsNullOrWhiteSpace(size)) ScaleValue=Convert.ToDouble(op);
                        if (!String.IsNullOrWhiteSpace(posX)) Left=Convert.ToDouble(posX);
                        if (!String.IsNullOrWhiteSpace(posY)) Top=Convert.ToDouble(posY);
                        win5.Opacity = Opacity;
                        win5.ApplicationScaleTransform.ScaleX = ScaleValue;
                        win5.ApplicationScaleTransform.ScaleY = ScaleValue;
                        rendering = false;
                    }
                    catch (Exception e)
                    {
                        string report = e.TargetSite + "//" + e.Message + "//" + e.InnerException + "//" + e.Source + "//" + e.StackTrace + "///" + Environment.OSVersion.Version.Major + "///" + Environment.OSVersion.Version.Minor;
                        var responseString = MainWindow.MakeRequest(new Uri(@"https://mtgarena.pro/mtg/donew.php"), new Dictionary<string, object> { { @"cmd", @"cm_errreport" }, { @"token", MainWindow.Usertoken }, { @"cm_errreport", report } });
                    }

                    RkApp.Close();
                }
            }
            catch (Exception ee)
            {
                MainWindow.ErrReport(ee);
            }
        }

        //Set overlay settings

        public void SetOverlayData()
        {
            try
            {
                string op = Opacity.ToString();
                string size = ScaleValue.ToString();
                string posX = Left.ToString();
                string posY = Top.ToString();

                RegistryKey RkApp = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\\MTGAProtracker", true);
                if (RkApp == null)
                {
                    RkApp = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\\MTGAProtracker", true);
                }

                RkApp.SetValue("op", op);
                RkApp.SetValue("size", size);
                RkApp.SetValue("posX", posX);
                RkApp.SetValue("posY", posY);

                RkApp.Close();
            }
            catch (Exception ee)
            {
                MainWindow.ErrReport(ee);
            }
        }

        //Load information about cards

        private void Loadcardsdb()
        {
            //
            if (cdb.Count == 0)
            {
                try
                {
                    Stream cardsdbstream = Application.GetResourceStream(new Uri(@"pack://application:,,,/CardsDB/cards_db_app.json")).Stream;
                    using (StreamReader reader = new StreamReader(cardsdbstream))
                    {
                        string cards_db = reader.ReadToEnd();
                        cdb = JsonConvert.DeserializeObject<Dictionary<int, Card>>(cards_db);
                        foreach (var cid in cdb)
                        {
                            if (!cdb_mtga_id.ContainsKey(cid.Value.Mtga_id)) cdb_mtga_id.Add(cid.Value.Mtga_id, cid.Key);
                        }
                    }
                    
                }
                catch (Exception ee)
                {
                    MainWindow.ErrReport(ee);
                }
            }
        }

        //Render deck (for user or enemy)

        private void renderdeck(Dictionary<int, int> carray, int ncards, string curmode, Dictionary<int, int> Udecklive = null)
        {
            if (Udecklive == null)
            {
                Udecklive = new Dictionary<int, int>();
            }

            foreach (var ucard in carray)
            {
                if (ucard.Key != -1)
                {
                    var manaparsed = new Dictionary<string, int>();
                    try
                    {
                        manaparsed = JsonConvert.DeserializeObject<Dictionary<string, int>>(cdb[ucard.Key].Mana);
                    }
                    catch (Exception ee)
                    {
                       // MainWindow.ErrReport(ee);
                    }

                    DropShadowEffect myDropShadowEffect = new DropShadowEffect()
                    {
                        Color = (Color)ColorConverter.ConvertFromString(@"#FF000000"),
                        BlurRadius = 1,
                        ShadowDepth = 1,
                        Direction = 320,
                        Opacity = 0.9
                    };

                    TextBlock txt = new TextBlock()
                    {
                        FontWeight = FontWeights.Bold,
                        Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(@"#"+ colors_font[cdb[ucard.Key].Colorindicator])),
                        Text = cdb[ucard.Key].Name,
                        FontSize = 14,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(5, 0, 0, 0),
                        TextTrimming = TextTrimming.CharacterEllipsis
                    };

                    if (Window1.ovlsettings.Font == 1)
                    {
                        txt.FontFamily = new FontFamily(@"Segoe UI");
                    }
                    else
                    {
                        txt.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#JaceBeleren");
                    }

                    TextBlock manatext = new TextBlock()
                    {
                        FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#Mana"),
                        HorizontalAlignment = HorizontalAlignment.Right,
                        Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(@"#" + colors_font[cdb[ucard.Key].Colorindicator])),
                        TextAlignment = TextAlignment.Right,
                        VerticalAlignment = VerticalAlignment.Center,
                    };

                    string output = "";

                    if (curmode == "me")
                    {
                        switch (MainWindow.ovlsettings.Leftdigit){
                            case 0:
                                output += (ucard.Value - (Udecklive.ContainsKey(ucard.Key) ? Udecklive[ucard.Key] : 0)).ToString();
                                break;
                            case 1:
                                output += ucard.Value;
                                break;
                            case 2:
                                output += Convert.ToString(Math.Round((double)100 * (ucard.Value - (Udecklive.ContainsKey(ucard.Key) ? Udecklive[ucard.Key] : 0)) / ncards)) + @"%";
                                break;
                            case 3:
                                output += "";
                                break;
                        }

                        if (output != "") output += "  ";

                        switch (MainWindow.ovlsettings.Rightdigit)
                        {
                            case 0:
                                output += (ucard.Value - (Udecklive.ContainsKey(ucard.Key) ? Udecklive[ucard.Key] : 0)).ToString();
                                break;
                            case 1:
                                output += ucard.Value;
                                break;
                            case 2:
                                output += Convert.ToString(Math.Round((double)100 * (ucard.Value - (Udecklive.ContainsKey(ucard.Key) ? Udecklive[ucard.Key] : 0)) / ncards)) + @"%";
                                break;
                            case 3:
                                output += "";
                                break;
                        }
                    }
                    else if (curmode == "opponent")
                    {
                        output = ucard.Value.ToString();
                    }
                    else if (curmode == "draft")
                    {
                        switch (MainWindow.ovlsettings.Leftdigitdraft)
                        {
                            case 0:
                                output += ucard.Value;
                                break;
                            case 1:
                                if (collection.ContainsKey(ucard.Key)) {
                                    output += collection[ucard.Key];
                                }
                                else
                                {
                                    output += "0";
                                }
                                break;
                            case 2:
                                output += "";
                                break;
                        }

                        if (output != "") output += "  ";

                        switch (MainWindow.ovlsettings.Rightdigitdraft)
                        {
                            case 0:
                                output += ucard.Value;
                                break;
                            case 1:
                                if (collection.ContainsKey(ucard.Key))
                                {
                                    output += collection[ucard.Key];
                                }
                                else
                                {
                                    output += "0";
                                }
                                break;
                            case 2:
                                output += "";
                                break;
                        }
                    }

                    cnums[curmode].Add(ucard.Key, new TextBlock()
                    {
                        Name = @"cnum_" + ucard.Key.ToString(),
                        Text = output,
                        FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#Beleren"),
                        Effect = myDropShadowEffect,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Center,
                        TextAlignment = TextAlignment.Center,
                        Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(@"#FFFFFFFF")),
                        Width = 45,
                        Margin = new Thickness(205, 0, 0, 0),
                        Padding = new Thickness(0, 2, 2, 0)
                    });

                    LinearGradientBrush brush = new LinearGradientBrush();


                    if (cdb[ucard.Key].Colorindicator != @"Multicolor")
                    {
                        brush = new LinearGradientBrush((Color)ColorConverter.ConvertFromString(@"#" + colors_dark[cdb[ucard.Key].Colorindicator]), (Color)ColorConverter.ConvertFromString(@"#" + colors_dark[cdb[ucard.Key].Colorindicator]), 90);

                        try
                        {
                            if (manaparsed.ContainsKey("Colorless"))
                            {
                                manatext.Text += icons_mana[manaparsed["Colorless"].ToString()];
                            }

                            foreach (var m in manaparsed)
                            {
                                if (m.Key != "Colorless")
                                {
                                    for (var mnum = 0; mnum < m.Value; mnum++)
                                    {
                                        manatext.Text += icons_mana[m.Key];
                                    }
                                }
                            }
                        }
                        catch (Exception ee)
                        {
                            //MainWindow.ErrReport(ee);
                        }
                    }
                    else
                    {
                        GradientStopCollection gradientStops = new GradientStopCollection();
                        var n = 0;
                        try
                        {
                            if (manaparsed.ContainsKey("Colorless"))
                            {
                                manatext.Text += icons_mana[manaparsed["Colorless"].ToString()];
                            }
                            foreach (var m in manaparsed)
                            {
                                string[] mkeys = m.Key.Split('/');
                                foreach (var mk in mkeys)
                                {
                                    if (mk != "Colorless")
                                    {
                                        if (n == 0)
                                        {
                                            gradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString(@"#" + colors_dark[mk]), 0));
                                            gradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString(@"#" + colors_dark[mk]), 0.4));
                                            n++;
                                        }
                                        else if (n == 1)
                                        {
                                            gradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString(@"#" + colors_dark[mk]), 0.6));
                                            gradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString(@"#" + colors_dark[mk]), 1));
                                        }
                                        
                                        for (var mnum = 0; mnum < m.Value; mnum++)
                                        {
                                            manatext.Text += icons_mana[mk];
                                        }
                                    }
                                }
                            }
                            brush.GradientStops = gradientStops;
                        }
                        catch (Exception ee)
                        {
                            //MainWindow.ErrReport(ee);
                        }
                    }

                    manatext.Width = manatext.Text.Length * 12;
                    txt.Width = 185 - manatext.Width;
                    manatext.Margin = new Thickness(txt.Width+2, 2, 0, 0);
                    
                    LinearGradientBrush grbr = new LinearGradientBrush((Color)ColorConverter.ConvertFromString(@"#" + colors_light[cdb[ucard.Key].Colorindicator]), (Color)ColorConverter.ConvertFromString(@"#" + colors_dark[cdb[ucard.Key].Colorindicator]), 90);

                    borders[curmode].Add(ucard.Key, new Border()
                    {
                        Name = @"C_" + ucard.Key.ToString(),
                        Width = 252,
                        Height = 27,
                        Background = new SolidColorBrush()
                        {
                            Color = (Color)ColorConverter.ConvertFromString(@"#36221e")
                        },
                        BorderThickness = new Thickness(0, 0, 0, 0),
                        CornerRadius = new CornerRadius(5),
                        Margin = new Thickness(0, topmargin[curmode], 0, 0),
                        Padding = new Thickness(2, 2, 2, 2),
                        VerticalAlignment = VerticalAlignment.Top,
                        HorizontalAlignment = HorizontalAlignment.Left
                    });

                    Border recinner = new Border()
                    {
                        Width = 200,
                        Height = 22,
                        Background = brush,
                        BorderBrush = grbr,
                        BorderThickness = new Thickness(2, 2, 2, 2),
                        CornerRadius = new CornerRadius(5),
                        Padding = new Thickness(2, 0, 0, 0),
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Left
                    };

                    Canvas cnvas = new Canvas();
                    Canvas cnvas_outer = new Canvas();

                    cnvas.Children.Add(txt);
                    cnvas.Children.Add(manatext);
                    recinner.Child = cnvas;

                    cnvas_outer.Children.Add(recinner);
                    cnvas_outer.Children.Add(cnums[curmode][ucard.Key]);

                    borders[curmode][ucard.Key].Child = cnvas_outer;

                    topmargin[curmode] += 27;

                    if (curmode == "me" || curmode == @"draft")
                    {
                        overlayme.Children.Add(borders[curmode][ucard.Key]);
                    }
                    else if (curmode == "opponent")
                    {
                        overlayopp.Children.Add(borders[curmode][ucard.Key]);
                    }
                    if(MainWindow.ovlsettings.Showcard) borders[curmode][ucard.Key].MouseEnter += new MouseEventHandler(Mouse_overcard);

                }
            }
        }

        //Render decklist

        private void renderdecklist(Deck[] darray, string curmode)
        {

            foreach (var udeck in darray)
            {
                    var manaparsed = new Dictionary<string, int>();
                    var Colorindicator = @"";
                    try
                    {
                        manaparsed = JsonConvert.DeserializeObject<Dictionary<string, int>>(udeck.Classstruct);
                        if (Colorindicator == @"")
                        {
                            Colorindicator = manaparsed.Keys.First();
                        }
                    }
                    catch (Exception e)
                    {
                        /*string report = e.TargetSite + "//" + e.Message + "//" + e.InnerException + "//" + e.Source + "//" + e.StackTrace + "///" + Environment.OSVersion.Version.Major + "///" + Environment.OSVersion.Version.Minor;
                        var responseString = MainWindow.MakeRequest(new Uri(@"https://mtgarena.pro/mtg/donew.php"), new Dictionary<string, object> { { @"cmd", @"cm_errreport" }, { @"token", MainWindow.Usertoken }, { @"cm_errreport", report }, { @"version", MainWindow.version }, { @"function", @"manaparsed" } });*/
                }

                    DropShadowEffect myDropShadowEffect = new DropShadowEffect()
                    {
                        Color = (Color)ColorConverter.ConvertFromString(@"#FF000000"),
                        BlurRadius = 1,
                        ShadowDepth = 1,
                        Direction = 320,
                        Opacity = 0.9
                    };

                 TextBlock txt = new TextBlock()
                 {
                     FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#JaceBeleren"),
                     FontWeight = FontWeights.Bold,
                     Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(@"#" + colors_font[Colorindicator])),
                     Text = udeck.Humanname,
                     FontSize = 14,
                     HorizontalAlignment = HorizontalAlignment.Left,
                     VerticalAlignment = VerticalAlignment.Center,
                     Margin = new Thickness(5, 0, 0, 0),
                     Width = 150,
                     TextTrimming = TextTrimming.CharacterEllipsis
                 };

                if (Window1.ovlsettings.Font == 1)
                {
                    txt.FontFamily = new FontFamily(@"Segoe UI");
                }
                else
                {
                    txt.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#JaceBeleren");
                }

                TextBlock manatext = new TextBlock()
                 {
                     FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#Mana"),
                     HorizontalAlignment = HorizontalAlignment.Right,
                     Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(@"#" + colors_font[Colorindicator])),
                     TextAlignment = TextAlignment.Right,
                     VerticalAlignment = VerticalAlignment.Center,
                     Width = 35,
                     Margin = new Thickness(155, 2, 0, 0)
                 };

                 string output = "";


                 output = udeck.perswinratio.ToString()+@" | "+udeck.wlnratio.ToString();
                 

                 cnums[curmode].Add(udeck.Id, new TextBlock()
                 {
                     Name = @"cnum_" + udeck.Id.ToString(),
                     Text = output,
                     FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#Beleren"),
                     Effect = myDropShadowEffect,
                     HorizontalAlignment = HorizontalAlignment.Left,
                     VerticalAlignment = VerticalAlignment.Center,
                     TextAlignment = TextAlignment.Center,
                     Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(@"#FFFFFFFF")),
                     Width = 45,
                     Margin = new Thickness(205, 0, 0, 0),
                     Padding = new Thickness(0, 2, 2, 0)
                 });

                var n = 0;
                foreach (var m in manaparsed)
                {
                    string[] mkeys = m.Key.Split('/');
                    foreach (var mk in mkeys)
                    {
                        if (mk != "Colorless" && mk!="Multicolor")
                        {
                            manatext.Text += icons_mana[mk];
                        }
                    }
                }
                 


                 LinearGradientBrush grbr = new LinearGradientBrush((Color)ColorConverter.ConvertFromString(@"#" + colors_light[Colorindicator]), (Color)ColorConverter.ConvertFromString(@"#" + colors_dark[Colorindicator]), 90);

                 borders[curmode].Add(udeck.Id, new Border()
                 {
                     Name = @"C_" + udeck.Id.ToString(),
                     Width = 252,
                     Height = 27,
                     Background = new SolidColorBrush()
                     {
                         Color = (Color)ColorConverter.ConvertFromString(@"#36221e")
                     },
                     BorderThickness = new Thickness(0, 0, 0, 0),
                     CornerRadius = new CornerRadius(5),
                     Margin = new Thickness(0, topmargin[curmode], 0, 0),
                     Padding = new Thickness(2, 2, 2, 2),
                     VerticalAlignment = VerticalAlignment.Top,
                     HorizontalAlignment = HorizontalAlignment.Left
                 });

                 Border recinner = new Border()
                 {
                     Width = 200,
                     Height = 22,
                     Background = new SolidColorBrush()
                     {
                         Color = (Color)ColorConverter.ConvertFromString(@"#22FFFFFF")
                     },
                     BorderBrush = grbr,
                     BorderThickness = new Thickness(2, 2, 2, 2),
                     CornerRadius = new CornerRadius(5),
                     Padding = new Thickness(2, 0, 0, 0),
                     VerticalAlignment = VerticalAlignment.Center,
                     HorizontalAlignment = HorizontalAlignment.Left
                 };

                 Canvas cnvas = new Canvas();
                 Canvas cnvas_outer = new Canvas();

                 cnvas.Children.Add(txt);
                 cnvas.Children.Add(manatext);
                 recinner.Child = cnvas;

                 cnvas_outer.Children.Add(recinner);
                 cnvas_outer.Children.Add(cnums[curmode][udeck.Id]);

                 borders[curmode][udeck.Id].Child = cnvas_outer;

                 topmargin[curmode] += 27;

                 overlayme.Children.Add(borders[curmode][udeck.Id]);
            }
        }

        //Set current mode: own cards, opponents, draft or list of decks

        public void Setmode(string modesetter)
        {
            mode = modesetter;
            if (mode == @"me" || mode == @"draft" || mode==@"decks")
            {
                overlayme.Visibility = Visibility.Visible;
                overlayopp.Visibility = Visibility.Hidden;
                if(mode == @"me" || mode == @"draft")
                {
                    bottommenu.Visibility = Visibility.Visible;
                }
                else
                {
                    bottommenu.Visibility = Visibility.Hidden;
                }
            }
            else if(mode == @"opponent")
            {
                overlayme.Visibility = Visibility.Hidden;
                overlayopp.Visibility = Visibility.Visible;
                bottommenu.Visibility = Visibility.Visible;
            }
            bottommenu.Margin = new Thickness(0, topmargin[mode], 0, 0);
        }

        //update of overlay. Key function where all action happens.

        public void updatelive()
        {
            wasshown = true;
            //Handling draft
            if (MainWindow.TheMatch.IsDrafting)
            {
                string curdrft = MainWindow.MakeRequest(new Uri(@"https://mtgarena.pro/mtg/donew.php"), new Dictionary<string, object> { { @"cmd", @"cm_getlivedraft" }, { @"uid", MainWindow.ouruid }, { @"token", MainWindow.Usertoken }, {@"cardsquery", JsonConvert.SerializeObject(MainWindow.TheMatch.Draftdeck) } });
                if (curdrft != @"ERRCONN")
                {
                    Dictionary<string, Dictionary<int, int>> curdrftparsed = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<int, int>>>(curdrft);
                    collection = curdrftparsed["col"];
                    PackOpening = MainWindow.TheMatch.DraftPack;
                    PickMacking = MainWindow.TheMatch.DraftPick;
                    overlayme.Children.Clear();
                    topmargin[@"draft"] = 20;
                    cnums[@"draft"].Clear();
                    borders[@"draft"].Clear();
                    try
                    {
                        renderdeck(curdrftparsed["eval"], 0, @"draft");
                    }
                    catch (Exception ee)
                    {
                        MainWindow.ErrReport(ee);
                    }

                    melabel.Text = @"Pick: " + (PickMacking + 1).ToString();
                    opponentlabel.Text = @"Pack: " + (PackOpening + 1).ToString();
                    Setmode(@"draft");
                    decksrendered = false;
                }
            }
            //Handling fight
            else if(MainWindow.TheMatch.IsFighting)
            {
                string curbtl = MainWindow.MakeRequest(new Uri(@"https://mtgarena.pro/mtg/donew.php"), new Dictionary<string, object> { { @"cmd", @"cm_getlivematch" }, { @"uid", MainWindow.ouruid }, { @"token", MainWindow.Usertoken } });
                if (curbtl != @"ERRCONN")
                {
                    Battle curbtlparsed = JsonConvert.DeserializeObject<Battle>(curbtl);
                    string setmode = @"";
                    curbtlparsed.Udecklive = MainWindow.TheMatch.Udeck;
                    curbtlparsed.Edeck = MainWindow.TheMatch.Edeck;
                    if (curbtlparsed.Edeckname != @"")
                    {
                        opponentdecklabel.Text = curbtlparsed.Edeckname;
                    }
                    if (curbtlparsed.Humanname != @"")
                    {
                        mydecklabel.Text = curbtlparsed.Humanname;
                    }
                    int ncards = curbtlparsed.Deckstruct.Sum(x => x.Value) - curbtlparsed.Udecklive.Sum(x => x.Value);

                    if (!(curbtlparsed.Udeck_fp == null))
                    {
                        if (matchplayingwe != MainWindow.TheMatch.Matchid)
                        {
                            matchplayingwe = MainWindow.TheMatch.Matchid;
                            deckplaying = curbtlparsed.Udeck_fp;
                            overlayme.Children.Clear();
                            topmargin[@"me"] = 20;
                            cnums[@"me"].Clear();
                            borders[@"me"].Clear();
                            mydecklabel.Text = @"";
                            oldUdecklive.Clear();
                            try
                            {
                                renderdeck(curbtlparsed.Deckstruct, ncards, @"me", curbtlparsed.Udecklive);
                                mode = "me";
                                setmode = @"me";
                            }
                            catch (Exception ee)
                            {
                                MainWindow.ErrReport(ee);
                            }
                        }
                        else
                        {
                            foreach (var ucard in curbtlparsed.Deckstruct)
                            {
                                if (ucard.Key != -1)
                                {
                                    int nc = 0;
                                    bool needanimate = false;
                                    string output = @"";
                                    switch (MainWindow.ovlsettings.Leftdigit)
                                    {
                                        case 0:
                                            nc = ucard.Value - (curbtlparsed.Udecklive.ContainsKey(ucard.Key) ? curbtlparsed.Udecklive[ucard.Key] : 0);
                                            output += nc.ToString();
                                            break;
                                        case 1:
                                            nc = ucard.Value;
                                            output += nc.ToString();
                                            break;
                                        case 2:
                                            nc = (int)Math.Round((double)100 * (ucard.Value - (curbtlparsed.Udecklive.ContainsKey(ucard.Key) ? curbtlparsed.Udecklive[ucard.Key] : 0)) / ncards) ;
                                            output += nc.ToString() + @"%";
                                            break;
                                        case 3:
                                            output += "";
                                            break;
                                    }

                                    if (output != "") output += "  ";

                                    switch (MainWindow.ovlsettings.Rightdigit)
                                    {
                                        case 0:
                                            output += (ucard.Value - (curbtlparsed.Udecklive.ContainsKey(ucard.Key) ? curbtlparsed.Udecklive[ucard.Key] : 0)).ToString();
                                            break;
                                        case 1:
                                            output += ucard.Value;
                                            break;
                                        case 2:
                                            output += Convert.ToString(Math.Round((double)100 * (ucard.Value - (curbtlparsed.Udecklive.ContainsKey(ucard.Key) ? curbtlparsed.Udecklive[ucard.Key] : 0)) / ncards) ) + @"%";
                                            break;
                                        case 3:
                                            output += "";
                                            break;
                                    }

                                    borders[@"me"][ucard.Key].Opacity = 1;
                                    cnums[@"me"][ucard.Key].Text = output;

                                    if (curbtlparsed.Udecklive.ContainsKey(ucard.Key))
                                    {

                                        if (!oldUdecklive.ContainsKey(ucard.Key))
                                        {
                                            needanimate = true;
                                            oldUdecklive.Add(ucard.Key, curbtlparsed.Udecklive[ucard.Key]);
                                        }
                                        else if (oldUdecklive[ucard.Key] != curbtlparsed.Udecklive[ucard.Key])
                                        {
                                            oldUdecklive[ucard.Key] = curbtlparsed.Udecklive[ucard.Key];
                                            needanimate = true;
                                        }
                                    }

                                    if (needanimate)
                                    {
                                            
                                        ColorAnimation animation = new ColorAnimation()
                                        {
                                            To = (Color)ColorConverter.ConvertFromString("#f9f5ec"),
                                            Duration = TimeSpan.FromSeconds(0.4),
                                            AutoReverse = true,
                                        };

                                        /*animation.Completed += (s, e) =>
                                        {
                                            borders[@"me"][ucard.Key].Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(@"#36221e"));
                                        };
                                        borders[@"me"][ucard.Key].Background = new SolidColorBrush()
                                        {
                                            Color = (Color)ColorConverter.ConvertFromString(@"#" + colors[cdb[ucard.Key].Colorindicator])
                                        };*/
                                        borders[@"me"][ucard.Key].Background.BeginAnimation(SolidColorBrush.ColorProperty, animation);
                                        setmode = @"me";
                                    }
                                    if (ucard.Value - (curbtlparsed.Udecklive.ContainsKey(ucard.Key) ? curbtlparsed.Udecklive[ucard.Key] : 0) == 0)
                                    {
                                        borders[@"me"][ucard.Key].Opacity = 0.6;
                                    }
                                }
                            }
                        }
                    }


                    if (!curbtlparsed.Edeck.ContainsKey(-1))
                    {
                        if (matchplayingthey != MainWindow.TheMatch.Matchid)
                        {
                            matchplayingthey = MainWindow.TheMatch.Matchid;
                            overlayopp.Children.Clear();
                            cnums[@"opponent"].Clear();
                            borders[@"opponent"].Clear();
                            topmargin[@"opponent"] = 20;
                            opponentdecklabel.Text = @"";
                            try
                            {
                                renderdeck(curbtlparsed.Edeck, 0, @"opponent");
                            }
                            catch (Exception ee)
                            {
                                MainWindow.ErrReport(ee);
                            }
                            setmode = "opponent";
                        }
                        else
                        {
                            foreach (var ucard in curbtlparsed.Edeck)
                            {
                                if (ucard.Key != -1)
                                {
                                    int highlight = 0;
                                    if (!cnums[@"opponent"].ContainsKey(ucard.Key))
                                    {
                                        try
                                        {
                                            renderdeck(new Dictionary<int, int>() { { ucard.Key, ucard.Value } }, 0, @"opponent");
                                        }
                                        catch (Exception ee)
                                        {
                                            MainWindow.ErrReport(ee);
                                        }
                                        highlight = ucard.Key;
                                    }

                                        string oldtext = cnums[@"opponent"][ucard.Key].Text;

                                        cnums[@"opponent"][ucard.Key].Text = ucard.Value.ToString();
                                        if (oldtext != ucard.Value.ToString() || highlight == ucard.Key)
                                        {
                                            var oldbg = borders[@"opponent"][ucard.Key].Background;
                                            ColorAnimation animation = new ColorAnimation()
                                            {
                                                To = (Color)ColorConverter.ConvertFromString("#f9f5ec"),
                                                Duration = TimeSpan.FromSeconds(0.4),
                                                AutoReverse = true,
                                            };

                                            animation.Completed += (s, e) =>
                                            {
                                                borders[@"opponent"][ucard.Key].Background = oldbg;
                                            };
                                            borders[@"opponent"][ucard.Key].Background = new SolidColorBrush()
                                            {
                                                Color = (Color)ColorConverter.ConvertFromString(@"#" + colors[cdb[ucard.Key].Colorindicator])
                                            };
                                            borders[@"opponent"][ucard.Key].Background.BeginAnimation(SolidColorBrush.ColorProperty, animation);
                                            setmode = @"opponent";
                                        }
                                }
                            }
                        }

                    }


                    if (setmode != @"" && MainWindow.ovlsettings.Autoswitch)
                    {
                        Setmode(setmode);
                    }
                    else
                    {
                        bottommenu.Visibility = Visibility.Visible;
                        bottommenu.Margin = new Thickness(0, topmargin[mode], 0, 0);
                    }
                    decksrendered = false;
                }
            }
            else
            {
                //Handling decks list rendering
                string decks = MainWindow.MakeRequest(new Uri(@"https://mtgarena.pro/mtg/donew.php"), new Dictionary<string, object> { { @"cmd", @"cm_getuserdecks" }, { @"uid", MainWindow.ouruid }, { @"token", MainWindow.Usertoken } });
                if (decks != @"ERRCONN")
                {
                    Deck[] decksparsed = JsonConvert.DeserializeObject<Deck[]>(decks);
                    if (decksparsed.Length > 0)
                    {
                        overlayme.Children.Clear();
                        topmargin[@"decks"] = 20;
                        cnums[@"decks"].Clear();
                        borders[@"decks"].Clear();
                        try
                        {
                            renderdecklist(decksparsed, @"decks");
                        }
                        catch (Exception ee)
                        {
                            MainWindow.ErrReport(ee);
                        }
                        decksrendered = true;
                    }

                    Setmode(@"decks");
                }
            }
        }

        public Window4()
        {
            InitializeComponent();
            Closing += Window4_Closing;
            StreamResourceInfo sriCurs = Application.GetResourceStream(
            new Uri("pack://application:,,,/Resources/testcur.cur"));
            Cursor = new Cursor(sriCurs.Stream);
            Loadcardsdb();
            if (MainWindow.ovlsettings.Showtimers)
            {
                Timer.WorkerSupportsCancellation = true;
                Timer.DoWork += Timer_DoWork;

                if (!Timer.IsBusy)
                {
                    Timer.RunWorkerAsync();
                }
            }

            GetOverlayData();

            /*Cardloader.DoWork += Cardloader_DoWork;
            if (!Cardloader.IsBusy)
            {
                Cardloader.RunWorkerAsync();
            }*/
        }

        private void Window4_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            Visibility = Visibility.Hidden;
        }

        private void Mouse_offcard(object sender, MouseEventArgs e)
        {
            win5.Hide();
        }

        //Display card on hover

        private void Mouse_overcard(object sender, MouseEventArgs e)
        {
            Border sndr = sender as Border;
            cidover = Convert.ToInt32(sndr.Name.Replace(@"C_",@""));
            location = sndr.PointToScreen(new Point(0, 0));
            location.X += (sndr.ActualWidth+50) * ScaleValue;
            string cardname = cdb[cidover].Pict;
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"MTGAproTracker");

            PresentationSource source = PresentationSource.FromVisual(this);
            Point targetPoints = source.CompositionTarget.TransformFromDevice.Transform(location);

            Point ourwindow = PointToScreen(new Point(0, 0));
            Point ourwindow_target = source.CompositionTarget.TransformFromDevice.Transform(ourwindow);
            double winbottom = ourwindow_target.Y + ActualHeight * ScaleValue;
            FileInfo nfo = new FileInfo(path + @"\Cards\" + cardname);
            if (!nfo.Exists || nfo.Length == 0)
            {
                if(!Directory.Exists(path + @"\Cards\"))
                {
                    Directory.CreateDirectory(path + @"\Cards\");
                }

                using (WebClient myWebClient = new WebClient())
                {
                    try
                    {
                        myWebClient.DownloadFileAsync(new Uri(@"https://mtgarena.pro/mtg/pict/" + cardname), path + @"\Cards\" + cardname);
                        myWebClient.DownloadFileCompleted += MyWebClient_DownloadCardCompleted;
                    }
                    catch (Exception ee)
                    {
                        MainWindow.ErrReport(ee);
                    }
                }
            }
            else
            {
                MyWebClient_DownloadCardCompleted(null, null);
            }

            win5.Left = targetPoints.X;
            win5.Top = targetPoints.Y - (((targetPoints.Y + win5.ActualHeight * ScaleValue) > winbottom && (targetPoints.Y - (win5.ActualHeight * ScaleValue)) > ourwindow_target.Y) ? (win5.ActualHeight * ScaleValue) : 0);
            if (!win5.IsVisible) win5.Show();

            // Close();
        }

        private void MyWebClient_DownloadCardCompleted(object sender, AsyncCompletedEventArgs e)
        {
            try
            {
                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"MTGAproTracker");
                string cardname = cdb[cidover].Pict;
                

                BitmapImage src = new BitmapImage();
                src.BeginInit();
                src.UriSource = new Uri(path + @"\Cards\" + cardname);
                src.EndInit();
                ImageBrush imageBrush = new ImageBrush()
                {
                    ImageSource = src,
                };
                win5.cardrenderer.Fill = imageBrush;
            }
            catch (Exception ee)
            {
               // MainWindow.ErrReport(ee);
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
            
            //e.Handled = false;
        }

        private void Window_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            //   Close();
            //double cursv = ScaleValue;
            //ScaleValue = (double)OnCoerceScaleValue(OverlayWindow, (cursv - 0.1));
        }

        #region ScaleValue Depdency Property
        public static readonly DependencyProperty ScaleValueProperty = DependencyProperty.Register("ScaleValue", typeof(double), typeof(Window4), new UIPropertyMetadata(1.0, new PropertyChangedCallback(OnScaleValueChanged), new CoerceValueCallback(OnCoerceScaleValue)));

        private static object OnCoerceScaleValue(DependencyObject o, object value)
        {
            Window4 mainWindow = o as Window4;
            if (mainWindow != null)
                return mainWindow.OnCoerceScaleValue((double)value);
            else
                return value;
        }

        private static void OnScaleValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Window4 mainWindow = o as Window4;
            if (mainWindow != null)
                mainWindow.OnScaleValueChanged((double)e.OldValue, (double)e.NewValue);
        }

        protected virtual double OnCoerceScaleValue(double value)
        {
            if (double.IsNaN(value))
                return 1.0f;

            value = Math.Max(0.1, value);
            return value;
        }

        protected virtual void OnScaleValueChanged(double oldValue, double newValue)
        {

        }

        public double ScaleValue
        {
            get
            {
                return (double)GetValue(ScaleValueProperty);
            }
            set
            {
                SetValue(ScaleValueProperty, value);
            }
        }
        #endregion

        private void Border_MouseUp(object sender, MouseButtonEventArgs e)
        {
            double cursv = ScaleValue;
            ScaleValue = (double)OnCoerceScaleValue(OverlayWindow, cursv - 0.05);
            win5.ApplicationScaleTransform.ScaleX = ScaleValue;
            win5.ApplicationScaleTransform.ScaleY = ScaleValue;
            SetOverlayData();
        }

        private void Border_MouseUp_1(object sender, MouseButtonEventArgs e)
        {
            double cursv = ScaleValue;
            ScaleValue = (double)OnCoerceScaleValue(OverlayWindow, cursv + 0.05);
            win5.ApplicationScaleTransform.ScaleX = ScaleValue;
            win5.ApplicationScaleTransform.ScaleY = ScaleValue;
            SetOverlayData();
        }

        private void Border_MouseUp_2(object sender, MouseButtonEventArgs e)
        {
            double opa = (Opacity - 0.05);
            if (opa < 0.3) opa = 0.3;
            Opacity = opa;
            win5.Opacity = Opacity;
            SetOverlayData();
        }

        private void Border_MouseUp_3(object sender, MouseButtonEventArgs e)
        {
            double opa = Opacity + 0.05;
            if (opa > 1) opa = 1;
            Opacity = opa;
            win5.Opacity = Opacity;
            SetOverlayData();
        }

        private void Grid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (overlay.IsVisible)
            {
                overlay.Visibility = Visibility.Hidden;
               // overlay.Height = 0;
                collapser.Text = @"+";
                //collapser_name.Text = @"MTGA Pro Tracker";
                sizegrid.Visibility = Visibility.Hidden;
                opacitygrid.Visibility = Visibility.Hidden;
            }
            else
            {
                overlay.Visibility = Visibility.Visible;
                //overlay.Height = topmargin[mode]+35;
                collapser.Text = @"-";
                //collapser_name.Text = @"";
                sizegrid.Visibility = Visibility.Visible;
                opacitygrid.Visibility = Visibility.Visible;
            }
        }

        private void Grid_MouseLeftButtonUp_1(object sender, MouseButtonEventArgs e)
        {

        }

        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            if (mode != @"draft")
            {
                updatelive();
                Setmode("me");
            }
        }

        private void Grid_MouseEnter_1(object sender, MouseEventArgs e)
        {
            if (mode != @"draft" && mode != @"decks")
            {
                updatelive();
                Setmode("opponent");
            }
        }

        //Timer ticking
        private void Timer_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

                while (!worker.CancellationPending)
                {
                    if (MainWindow.TheMatch.IsFighting && MainWindow.TheMatch.DecisionPlayer!=0)
                    {
                        MainWindow.TheMatch.Timers[MainWindow.TheMatch.DecisionPlayer]++;
                        Dispatcher.BeginInvoke(new ThreadStart(delegate
                        {
                            int minutes = (int)Math.Floor((double)MainWindow.TheMatch.Timers[MainWindow.TheMatch.DecisionPlayer] / 60);
                            int seconds = MainWindow.TheMatch.Timers[MainWindow.TheMatch.DecisionPlayer] % 60;
                            string timer = ((minutes < 10) ? "0" : "") + Convert.ToString(minutes) + ":" + ((seconds < 10) ? "0" : "") + Convert.ToString(seconds);
                            if (MainWindow.TheMatch.DecisionPlayer == MainWindow.TheMatch.Teamid)
                            {
                                melabel.Text = @"Me "+timer;
                            }
                            else
                            {
                                opponentlabel.Text = timer+@" Them";
                            }
                        }));
                    }

                    Thread.Sleep(1000);
                }

        }

        private void Cardloader_DoWork(object sender, DoWorkEventArgs e)
        {
            foreach (var ca in cdb)
            {
                string cardname = ca.Value.Pict;
                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"MTGAproTracker");
                FileInfo nfo = new FileInfo(path + @"\Cards\" + cardname);
                if (!nfo.Exists || nfo.Length==0)
                {
                    if (!Directory.Exists(path + @"\Cards\"))
                    {
                        Directory.CreateDirectory(path + @"\Cards\");
                    }

                    using (WebClient myWebClient = new WebClient())
                    {
                        try
                        {
                            myWebClient.DownloadFileAsync(new Uri(@"https://mtgarena.pro/mtg/pict/" + cardname), path + @"\Cards\" + cardname);
                        }
                        catch (Exception ee)
                        {
                            MainWindow.ErrReport(ee);
                        }
                    }
                    Thread.Sleep(500);
                } 
            }
        }

        private void OverlayWindow_LocationChanged(object sender, EventArgs e)
        {
            if (!rendering)
            {
                SetOverlayData();
            }
        }

        private void OverlayWindow_ContentRendered(object sender, EventArgs e)
        {
            //GetOverlayData();
        }

        #region Hotkeys

        [DllImport("User32.dll")]
        private static extern bool RegisterHotKey(
        [In] IntPtr hWnd,
        [In] int id,
        [In] uint fsModifiers,
        [In] uint vk);

        [DllImport("User32.dll")]
        private static extern bool UnregisterHotKey(
            [In] IntPtr hWnd,
            [In] int id);

        private HwndSource _source;
        private const int HOTKEY_Q = 9000;
        private const int HOTKEY_W = 9001;
        private const int HOTKEY_T = 9002;
        private const int HOTKEY_O = 9003;

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var helper = new WindowInteropHelper(this);
            _source = HwndSource.FromHwnd(helper.Handle);
            _source.AddHook(HwndHook);
            if (MainWindow.ovlsettings.Hotkeys)
            {
                RegisterHotKey();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            _source.RemoveHook(HwndHook);
            _source = null;
            UnregisterHotKey();
            base.OnClosed(e);
        }

        private void RegisterHotKey()
        {
            var helper = new WindowInteropHelper(this);
            const uint VK_Q = 0x51;
            const uint VK_W = 0x57;
            const uint VK_T = 0xC0;
            const uint VK_O = 0x4F;
            const uint MOD_ALT = 0x0001;
            RegisterHotKey(helper.Handle, HOTKEY_Q, MOD_ALT, VK_Q);
            RegisterHotKey(helper.Handle, HOTKEY_W, MOD_ALT, VK_W);
            RegisterHotKey(helper.Handle, HOTKEY_T, MOD_ALT, VK_T);
            RegisterHotKey(helper.Handle, HOTKEY_O, MOD_ALT, VK_O);
        }

        private void UnregisterHotKey()
        {
            var helper = new WindowInteropHelper(this);
            UnregisterHotKey(helper.Handle, HOTKEY_Q);
            UnregisterHotKey(helper.Handle, HOTKEY_W);
            UnregisterHotKey(helper.Handle, HOTKEY_T);
            UnregisterHotKey(helper.Handle, HOTKEY_O);
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;
            switch (msg)
            {
                case WM_HOTKEY:
                    switch (wParam.ToInt32())
                    {
                        case HOTKEY_Q:
                            OnHotKeyPressed(HOTKEY_Q);
                            break;
                        case HOTKEY_W:
                            OnHotKeyPressed(HOTKEY_W);
                            break;
                        case HOTKEY_T:
                            OnHotKeyPressed(HOTKEY_T);
                            break;
                        case HOTKEY_O:
                            OnHotKeyPressed(HOTKEY_O);
                            break;
                    }
                    handled = true;
                    break;
            }
            return IntPtr.Zero;
        }

        private void OnHotKeyPressed(int hkid)
        {
            switch (hkid)
            {
                case HOTKEY_Q:
                    if (mode != @"draft")
                    {
                        updatelive();
                        Setmode("me");
                    }
                    break;
                case HOTKEY_W:
                    if (mode != @"draft" && mode != @"decks")
                    {
                        updatelive();
                        Setmode("opponent");
                    }
                    break;
                case HOTKEY_T:
                    if (overlay.IsVisible)
                    {
                        overlay.Visibility = Visibility.Hidden;
                        // overlay.Height = 0;
                        collapser.Text = @"+";
                        //collapser_name.Text = @"MTGA Pro Tracker";
                    }
                    else
                    {
                        overlay.Visibility = Visibility.Visible;
                        //overlay.Height = topmargin[mode]+35;
                        collapser.Text = @"-";
                       // collapser_name.Text = @"Collapse";
                    }
                    break;
                case HOTKEY_O:
                    if (IsVisible == true)
                    {
                        Hide();
                        windowhidden = true;
                    }
                    else
                    {
                        Show();
                        windowhidden = false;
                    }
                    break;
            }
        }

        #endregion
    }
}

