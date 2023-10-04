namespace Squiggles.DialogicSharp.Examples;

using Godot;
using Squiggles.DialogicSharp;

public partial class LoadTimelineFromResource : Control {

  [Export] private Resource _timelineResource;
  private Dialogic _dialogic;
  public override void _Ready() {
    _dialogic = new(this);
    _dialogic.Start(_timelineResource);
    _dialogic.TimelineEnded += () => GetTree().Quit();
  }
}
