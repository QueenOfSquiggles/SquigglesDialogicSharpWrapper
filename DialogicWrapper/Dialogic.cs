namespace Squiggles.DialogicSharp;

using System;
using Godot;

public class Dialogic : IDisposable {
  //
  //  Main Wrapper Creation Stuff
  //
  // node reference to dialogic singleton
  private readonly Node _dialogic;
  // a dict of signals so the lambda callables can be disconnected when this
  private readonly System.Collections.Generic.Dictionary<string, Callable> _signalRefs;
  // a dict of string names to reduce allocations
  private readonly System.Collections.Generic.Dictionary<string, StringName> _names;
  /// <summary>
  /// Creates a wrapper around the gdscript Dialogic singleton. Implements <see cref="IDisposable"/> to allow proper handling of disconnecting and disposal of resources automagically. (sparkles and shit)
  /// </summary>
  /// <param name="node">Any node that is currently part of the scene tree. Used to get a reference to the Dialogic autoload singleton</param>
  public Dialogic(Node node) {
    _dialogic = node.GetNode("/root/Dialogic");

    _signalRefs = new() {
      // callable lambdas are cached to allow disconnecting when this object is disposed. Prevents huge amounts of dead signal connections
      {"dialogic_paused", Callable.From(() => DialogicPaused?.Invoke())},
      {"dialogic_resumed", Callable.From(() => DialogicResumed?.Invoke())},
      {"state_changed", Callable.From<int>((state) => StateChanged?.Invoke(state))},
      {"timeline_ended", Callable.From(() => TimelineEnded?.Invoke())},
      {"timeline_started", Callable.From(() => TimelineStarted?.Invoke())},
      {"event_handled", Callable.From<Resource>((res) => EventHandled?.Invoke(res))},
      {"signal_event", Callable.From<string>((argz) => SignalEvent?.Invoke(argz))},
      {"text_signal", Callable.From<string>((argz) => TextSignal?.Invoke(argz))},
    };
    _names = new() {
      // strings are allocated as string names here, and never again to prevent any performance drop from frequent calls.
      {"dialogic_paused", "dialogic_paused"},
      {"dialogic_resumed", "dialogic_resumed"},
      {"state_changed", "state_changed"},
      {"timeline_ended", "timeline_ended"},
      {"timeline_started", "timeline_started" },
      {"event_handled", "event_handled"},
      {"signal_event", "signal_event"},
      {"text_signal", "text_signal"},
      {"current_timeline", "current_timeline"},
      {"current_state", "current_state"},
      {"paused", "paused"},
      {"start", "start"},
    };

    foreach (var entry in _signalRefs) {
      _dialogic.Connect(_names[entry.Key], entry.Value);
    }
  }

  /// <summary>
  /// Disposes of all used resources, including disconnecting from signals and such. Called automatically by the garbage collector.
  /// </summary>
  public void Dispose() {
    GC.SuppressFinalize(this);
    // disconnect from dialogic singleton
    foreach (var entry in _signalRefs) {
      _dialogic.Disconnect(_names[entry.Key], entry.Value);
    }
  }

  //
  //  Wrapped properties, functions, and more???
  //

  // mirrored enums
  public enum States { IDLE, SHOWING_TEXT, ANIMATING, AWAITING_CHOICE, WAITING }

  // Signals
  public event Action DialogicPaused;
  public event Action DialogicResumed;
  public event Action<int> StateChanged;
  public event Action TimelineEnded;
  public event Action TimelineStarted;
  public event Action<Resource> EventHandled;
  public event Action<string> SignalEvent;
  public event Action<string> TextSignal;

  // properties

  public string CurrentTimeline => _dialogic?.Get(_names["current_timeline"]).AsString();
  public States CurrentState => (States)(_dialogic?.Get(_names["current_state"]).AsInt32() ?? 0);
  public bool Paused => _dialogic?.Get(_names["paused"]).AsBool() ?? false;


  // end user functions
  public void Start(string timeline) => _dialogic?.Call(_names["start"], timeline);
  public void Start(Resource timeline) => _dialogic?.Call(_names["start"], timeline);
  public void Start(Timeline timeline) => _dialogic?.Call(_names["start"], timeline.Get());
  // TODO: would more implemented functions be helpful to be wrapped?

}
