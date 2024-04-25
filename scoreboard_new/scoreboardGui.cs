using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.Config;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace scoreboard
{
    public class ScoreboardGui : GuiDialog
    {
        public override string ToggleKeyCombinationCode => "scoreboardgui";
        public string[] Stats { get; set; }
        private static int Page = 0;
        private static string Tab = "Deaths";
        public int tabPages;
        public bool isFresh;
        public bool isNew;
        public ScoreboardGui(ICoreClientAPI capi, string[] leaderStats, bool fresh, bool newGui) : base(capi)
        {
            Stats = leaderStats;
            
            isFresh = fresh;
            if (newGui)
            {
                //capi.Logger.Debug("I am a new gui");
                Tab = "Deaths";
                Page = 0;
            }
            SetupDialog();
        }

        private string TrimToCol(string s, int limit)
        {

            int total = 0;
            string trimmed = "";
            for (int i = 0; i < s.Length; i++)
            {
                string c = s[i].ToString();
                if (CharSizes.ContainsKey(c))
                {
                    total += CharSizes[c];
                }
                else total += 19;
                if (total < limit)
                {
                    trimmed += c;
                }
            }
            return trimmed;
        }

        public string ENotation(string s)
        {
            int number = Convert.ToInt32 (s);
            /*Random r = new Random();
            number = r.Next(1000000, 1000000000);*/

            
            string eS = number.ToString("G2", CultureInfo.InvariantCulture).Replace("+", "").Replace("E0", "E");
            return eS;

        }
        private string GetTextCol(string key)
        {
            string text = "";
            for (int i = 0; i < (Stats.Length - 2) / 2; i++)
            {
                if (key == "NAME")
                {
                    //use trimToCol here
                    //use gererate random string to test
                    string nameText = Stats[i * 2 + 0];
                    //nameText = GenerateRandomString(25);
                    nameText = TrimToCol(nameText, 450);
                    if (nameText == "Empty") nameText = "-";
                    text += nameText + "\n";
                }
                else if (key == "VALUE")
                {
                    //test if it's over 6 char
                    //replace with scientific notation 1.0e4
                    string valueText = Stats[i * 2 + 1];
                    //valueText = "9999999";
                    if (valueText == "0") valueText = "";
                    if (valueText.Length > 6) valueText = ENotation(valueText);
                    text += valueText + "\n";
                }

            }
            return text;
        }
        private void SetupDialog()
        {
            int textBoxHeight = 380;
            ElementBounds dialogBounds = ElementStdBounds.AutosizedMainDialog.WithAlignment(EnumDialogArea.CenterMiddle);
            ElementBounds tabBounds = ElementBounds.Fixed(10, 30, 150, textBoxHeight).WithFixedAlignmentOffset(-20, 0);
            ElementBounds insetBounds = ElementBounds.Fixed(150, 30, 200, textBoxHeight);
            ElementBounds textBounds1 = ElementBounds.Fixed(10, 0, 390, textBoxHeight);
            ElementBounds textBounds2 = ElementBounds.Fixed(400, 0, 110, textBoxHeight);
            ElementBounds descInsetBounds = ElementBounds.Fixed(150, textBoxHeight + 60, 505, 50);
            ElementBounds descBounds = ElementBounds.Fixed(160, textBoxHeight + 65, 460, 50);
            ElementBounds leftBtnBounds = ElementBounds.Fixed(0, textBoxHeight + 55, 60, 60);
            ElementBounds rightBtnBounds = ElementBounds.Fixed(70, textBoxHeight + 55, 60, 60);
            ElementBounds bgBounds = ElementBounds.Fill.WithFixedPadding(GuiStyle.ElementToDialogPadding, 20.0);
            bgBounds.BothSizing = ElementSizing.FitToChildren;
            insetBounds.BothSizing = ElementSizing.FitToChildren;
            bgBounds.WithChildren(insetBounds);
            bgBounds.WithChildren(tabBounds);
            bgBounds.WithChildren(leftBtnBounds);
            bgBounds.WithChildren(rightBtnBounds);
            bgBounds.WithChildren(descBounds);
            bgBounds.WithChildren(descInsetBounds);
            insetBounds.WithChildren(textBounds1);
            insetBounds.WithChildren(textBounds2);

            //create the dialog
            SingleComposer = capi.Gui.CreateCompo("leaderstats", dialogBounds)
                .AddShadedDialogBG(bgBounds)
                .AddDialogTitleBar("Leaderstats Table", OnTitleBarCloseClicked)
                .AddInset(insetBounds, 4, 0.85f)
                .AddInset(descInsetBounds, 4, 0.85f)
                .AddDynamicText("empty", CairoFont.WhiteMediumText(), textBounds1, "text1")
                .AddDynamicText("empty", CairoFont.WhiteMediumText(), textBounds2, "text2")
                .AddDynamicText("Description goes here", CairoFont.WhiteMediumText(), descBounds, "desc")
                .AddButton("<", OnButtonLeftClick, leftBtnBounds)
                .AddButton(">", OnButtonRightClick, rightBtnBounds)
                .AddVerticalTabs(GenerateTabs(out int currentTab), tabBounds, OnTabClicked, "tabs")
                .Compose()
            ;
            PopulateFields();
        }
        public void PopulateFields()
        {
            if (Stats == null) return;

            //scrollbar.CurrentYPosition = posY;
            GuiElementDynamicText text1Elem = SingleComposer.GetDynamicText("text1");
            string text1 = GetTextCol("NAME");
            text1Elem.SetNewText(text1);
            GuiElementDynamicText text2Elem = SingleComposer.GetDynamicText("text2");
            string text2 = GetTextCol("VALUE");
            text2Elem.SetNewText(text2);
            GuiElementDynamicText descElem = SingleComposer.GetDynamicText("desc");
            descElem.SetNewText(Stats[20]);
            tabPages = Int32.Parse(Stats[21]);
           // capi.Logger.Debug("Setting new tabPages {0}", tabPages);
            GuiElementVerticalTabs tabsEl = SingleComposer.GetVerticalTab("tabs");
            if (Tab == "Deaths") tabsEl.SetValue(0);
            else if (Tab == "Blocks") tabsEl.SetValue(1);
            else if (Tab == "Crafting/Smithing") tabsEl.SetValue(2);
            else if (Tab == "Server") tabsEl.SetValue(3);
            else tabsEl.SetValue(4);

        }
        private bool OnButtonLeftClick()
        {
           // capi.Logger.Debug("Left btn click");
           // capi.Logger.Debug("PAGE BEFORE CLICK{0}", Page.ToString());
            Page--;
            if (Page < 0) Page = tabPages - 1;
          //  capi.Logger.Debug("PAGE AFTER CLICK{0}", Page.ToString());
            RequestPage(Page);
            return true;
        }
        private bool OnButtonRightClick()
        {
          //  capi.Logger.Debug("Right btn click");
            Page++;
            if (Page > tabPages - 1) Page = 0;
            RequestPage(Page);
            return true;
        }

        
        private void OnTabClicked(int tabIndex, GuiTab clickedTab)
        {
            //capi.ShowChatMessage("THIS TAB WAS CLICKED: "+clickedTab.Name);
            Tab = clickedTab.Name;
            
            if (isFresh) {
                isFresh = false;
            }
            else
            {
                Page = 0;
                RequestPage(Page);
               
            };
        }
        
        private void OnTitleBarCloseClicked()
        {
            TryClose();
        }
        private GuiTab[] GenerateTabs(out int currentTab)
        {
            GuiTab[] tabs = new GuiTab[5];

            tabs[0] = new GuiTab()
            {
                DataInt = 0,
                Name = "Deaths"
            };

            tabs[1] = new GuiTab()
            {
                DataInt = 1,
                Name = "Blocks"

            };
            tabs[2] = new GuiTab()
            {
                DataInt = 2,
                Name = "Crafting/Smithing"
            };
            tabs[3] = new GuiTab()
            {
                DataInt = 3,
                Name = "Server"
            };
            tabs[4] = new GuiTab()
            {
                DataInt = 4,
                Name = "Misc"
            };
            currentTab = 2;


            return tabs;
        }

        private Dictionary<string, int> CharSizes = new Dictionary<string, int>()
        {
            {"0", 20},
            {"1", 15},
            {"2", 20},
            {"3", 20},
            {"4", 20},
            {"5", 20},
            {"6", 20},
            {"7", 20},
            {"8", 20},
            {"9", 20},
            {"a", 20},
            {"b", 20},
            {"c", 19},
            {"d", 19},
            {"e", 20},
            {"f", 12},
            {"g", 19},
            {"h", 19},
            {"i", 6},
            {"j", 6},
            {"k", 20},
            {"l", 6},
            {"m", 31},
            {"n", 19},
            {"o", 21},
            {"p", 20},
            {"q", 19},
            {"r", 14},
            {"s", 18},
            {"t", 11},
            {"u", 19},
            {"v", 19},
            {"w", 28},
            {"x", 19},
            {"y", 19},
            {"z", 19},
            {"A", 26},
            {"B", 24},
            {"C", 27},
            {"D", 27},
            {"E", 24},
            {"F", 22},
            {"G", 28},
            {"H", 25},
            {"I", 7},
            {"J", 17},
            {"K", 26},
            {"L", 21},
            {"M", 30},
            {"N", 25},
            {"O", 29},
            {"P", 25},
            {"Q", 29},
            {"R", 28},
            {"S", 24},
            {"T", 23},
            {"U", 25},
            {"V", 26},
            {"W", 37},
            {"X", 26},
            {"Y", 26},
            {"Z", 23},
            {"+", 21},
            {"_", 22},
            {"&", 25},
            {"^", 17},
            {"%", 33},
            {"$", 20},
            {"#", 21},
            {"@", 39},
            {"!", 8}
        };
        static string GenerateRandomString(int length)
        {
            // Define the characters from which the random string will be generated
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            // Initialize a Random object
            Random random = new Random();
            // Use a StringBuilder to efficiently build the random string
            var randomString = new System.Text.StringBuilder(length);
            // Generate random characters and append them to the StringBuilder
            for (int i = 0; i < length; i++)
            {
                randomString.Append(chars[random.Next(chars.Length)]);
            }
            // Convert StringBuilder to string and return
            return randomString.ToString();
        }
        private void RequestPage(int page)
        {
           // capi.Logger.Debug("REQUESTING TAB {0} PAGE {1}", Tab, page.ToString());

            capi.Network.GetChannel("stat_request").SendPacket(new StatRequest()
            {
                response = "GET_SCORES",
                pageID = page,
                tabID = Tab,
                fresh = true,
                newGui = false
            });
        }
    }

}
