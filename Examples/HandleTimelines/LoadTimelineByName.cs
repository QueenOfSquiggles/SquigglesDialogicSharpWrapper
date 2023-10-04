namespace Squiggles.DialogicSharp.Examples;

using Godot;
using Squiggles.DialogicSharp;

public partial class LoadTimelineByName : Control {

  [Export] private string _timelineName;
  private Dialogic _dialogic;
  public override void _Ready() {
    _dialogic = new(this);
    _dialogic.Start(_timelineName);
    _dialogic.TimelineEnded += () => GetTree().Quit();
  }
}
