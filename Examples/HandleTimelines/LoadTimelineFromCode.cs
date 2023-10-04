namespace Squiggles.DialogicSharp.Examples;

using Godot;

public partial class LoadTimelineFromCode : Control {
  private Dialogic _dialogic;

  // Not currently working :/
  // Need to figure out why. No errors occur???
  public override void _Ready() {
    _dialogic = new(this);

    _dialogic.Start(new Timeline() // builder class is useful for chaining events which can create robust programmatic timeline building!
      .AddEvent(new Timeline.EventText("This is a line of text", null, ""))
      // Refer to Timeline class for the available Event** records.
      .AddEvent(new Timeline.EventText("Oh golly gee [wave]another one![/wave]", null, ""))
      // null and "" mean that we have no character and no specified portrait. But these could easily load a defined character and specific character portrait!!!
      .AddEvent(new Timeline.EventText("That's right BBCode works in this too!!!", null, ""))
    );
    _dialogic.TimelineEnded += () => GetTree().Quit();
  }
}
