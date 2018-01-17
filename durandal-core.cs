public Program()

{

// The constructor, called only once every session and
// always before any other method is called. Use it to
// initialize your script.
//
// The constructor is optional and can be removed if not
// needed.
//
// It's recommended to set RuntimeInfo.UpdateFrequency
// here, which will allow your script to run itself without a
// timer block.

}



public void Save()

{

// Called when the program needs to save its state. Use
// this method to save your state to the Storage field
// or some other means.
//
// This method is optional and can be removed if not
// needed.

}



public void Main(string argument, UpdateType updateSource)

{

// The main entry point of the script, invoked every time
// one of the programmable block's Run actions are invoked,
// or the script updates itself. The updateSource argument
// describes where the update came from.
//
// The method itself is required, but the arguments above
// can be removed if not needed.

  string ERR_TXT = "";

  // Find all Reactors on Currnet Grid Matrix
  List<IMyTerminalBlock> r = new List<IMyTerminalBlock>();
  GridTerminalSystem.GetBlocksOfType<IMyReactor>(r);
  if(r.Count == 0) {
    ERR_TXT += "no Large Reactors found!\n";
  }

 float maxPwr = 0.0f;
 float usePwr = 0.0f;

 var doDoor = GridTerminalSystem.GetBlockWithName("ProgramRoomDoor") as IMyDoor;
 var testLight = GridTerminalSystem.GetBlockWithName("TestLight") as IMyInteriorLight;
 var console = GridTerminalSystem.GetBlockWithName("Console") as IMyTextPanel;

 var hangarVent = GridTerminalSystem.GetBlockWithName("Hangar Vent") as IMyAirVent;
 var hangarConsole = GridTerminalSystem.GetBlockWithName("hangarConsole") as IMyTextPanel;

 var msd = GridTerminalSystem.GetBlockWithName("BridgeMSD") as IMyTextPanel;

if(doDoor.Status == DoorStatus.Open)
{
    // testLight. ApplyAction("OnOff_On");
    console.WritePublicText("Open", false);
    console.SetValue("BackgroundColor",new Color(0,0,0));
} else {
    // testLight.ApplyAction("OnOff_Off");
    console.WritePublicText("Closed", false);
    console.SetValue("BackgroundColor",new Color(255,0,0));
}

if(hangarVent.CanPressurize == true)
 {
    hangarConsole.WritePublicText("Hangar Pressurized", false);
    hangarConsole.SetValue("BackgroundColor", new Color(0,255,0));
 } else {
    hangarConsole.WritePublicText("Hangar Depressurized", false);
    hangarConsole.SetValue("BackgroundColor", new Color(255,0,0));
 }


 for(int i = 0; i < reactor0.Count; i++) {
     maxPwr = getExtraFieldFloat(reactor0[i], "Max Output: (\\d+\\.?\\d*) (\\w?)W");
     usePwr = getExtraFieldFloat(reactor0[i], "Max Output:.*Current Output: (\\d+\\.?\\d*) (\\w?)W");
 }
 msd.WritePublicText("Power Usage: "+powerFormat(usePwr)+"/"+powerFormat(maxPwr)+"", false);
 msd.ShowPublicTextOnScreen();
}



const string MULTIPLIERS = ".kMGTPEZY";
int POWER_PRECISION = 2;

float getExtraFieldFloat(IMyTerminalBlock block, string regexString) {
  System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(regexString, System.Text.RegularExpressions.RegexOptions.Singleline);

  float result = 0.0f;
  double parsedDouble;
  System.Text.RegularExpressions.Match match = regex.Match(block.DetailedInfo);

  if (match.Success) {
    if (Double.TryParse(match.Groups[1].Value, out parsedDouble)) {
      result = (float) parsedDouble;
    }
    if(MULTIPLIERS.IndexOf(match.Groups[2].Value) > -1) {
      result = result * (float) Math.Pow(1000.0, MULTIPLIERS.IndexOf(match.Groups[2].Value));
    }
  }

  return result;
}

string powerFormat(float power) {
  int counter = 0;
  while (power > 1000.0) {
    power = power / 1000;
    counter++;
  }

  string zeroes = "";
  for (int i = 0; i < POWER_PRECISION && i < 9; i++) {
    zeroes += "0";
  }
  
  return "" + Math.Round((double)power,POWER_PRECISION).ToString("##0."+zeroes) + " " + MULTIPLIERS.Substring(counter,1) + "W";
}
