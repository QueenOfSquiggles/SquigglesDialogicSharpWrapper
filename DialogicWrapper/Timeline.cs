namespace Squiggles.DialogicSharp;

using System;
using System.Collections.Generic;
using Godot;

/// <summary>
/// A builder class for constructing `DialogicTimeline` resources
/// </summary>
[Obsolete("This class is currently under development and does not currently work as expected")]
public class Timeline {

  private readonly List<Event> _events = new();

  public Timeline Raw(string rawCode) {
    _events.Add(new EventRaw(rawCode));
    return this;
  }
  public Timeline AddEvent(Event @event) {
    _events.Add(@event);
    return this;
  }


  /// <summary>
  /// Combines all currently built events into a single timeline and returns the timeline resource.
  /// </summary>
  /// <returns>a resource instance of the timeline. Since it's a GDScript class, that's the best we can do</returns>
  public Resource Get() {
    var timelineRes = new Resource();
    timelineRes.SetScript(GD.Load<Script>("res://addons/dialogic/Resources/timeline.gd"));

    var eventsArr = new Godot.Collections.Array();
    foreach (var e in _events) {
      if (e is EventRaw er) {
        eventsArr.Add(er.RawTextCode); // oly case that isn't stored as a resource type
      }
      else {
        var eventRes = new Resource();
        eventRes.SetScript(GetScriptFor(e));
        e.Apply(ref eventRes);
        eventsArr.Add(eventRes);
      }
    }
    timelineRes.Set("events", eventsArr);
    return timelineRes;
  }

  private static Script GetScriptFor(Event @event) {
    var resource = @event switch {
      EventText => GD.Load<Script>("res://addons/dialogic/Modules/Text/event_text.gd"),
      EventWaitInput => GD.Load<Script>("res://addons/dialogic/Modules/WaitInput/event_wait_input.gd"),
      EventWait => GD.Load<Script>("res://addons/dialogic/Modules/Wait/event_wait.gd"),
      EventVoice => GD.Load<Script>("res://addons/dialogic/Modules/Voice/event_voice.gd"),
      EventVariable => GD.Load<Script>("res://addons/dialogic/Modules/Variable/event_variable.gd"),
      EventTextInput => GD.Load<Script>("res://addons/dialogic/Modules/TextInput/event_text_input.gd"),
      EventSignal => GD.Load<Script>("res://addons/dialogic/Modules/Signal/event_signal.gd"),
      EventSetting => GD.Load<Script>("res://addons/dialogic/Modules/Settings/event_setting.gd"),
      EventSave => GD.Load<Script>("res://addons/dialogic/Modules/Save/event_save.gd"),
      EventReturn => GD.Load<Script>("res://addons/dialogic/Modules/Jump/event_return.gd"),
      EventLabel => GD.Load<Script>("res://addons/dialogic/Modules/Jump/event_label.gd"),
      EventJump => GD.Load<Script>("res://addons/dialogic/Modules/Jump/event_jump.gd"),
      EventHistory => GD.Load<Script>("res://addons/dialogic/Modules/History/event_history.gd"),
      EventGlossary => GD.Load<Script>("res://addons/dialogic/Modules/Glossary/event_glossary.gd"),
      EventEndBranch => GD.Load<Script>("res://addons/dialogic/Modules/Core/event_end_branch.gd"),
      EventEnd => GD.Load<Script>("res://addons/dialogic/Modules/Core/event_end_branch.gd"),
      EventCondition => GD.Load<Script>("res://addons/dialogic/Modules/Condition/event_condition.gd"),
      EventComment => GD.Load<Script>("res://addons/dialogic/Modules/Comment/event_comment.gd"),
      EventChoice => GD.Load<Script>("res://addons/dialogic/Modules/Choice/event_choice.gd"),
      EventPosition => GD.Load<Script>("res://addons/dialogic/Modules/Character/event_position.gd"),
      EventCharacter => GD.Load<Script>("res://addons/dialogic/Modules/Character/event_character.gd"),
      EventCallNode => GD.Load<Script>("res://addons/dialogic/Modules/CallNode/event_call_node.gd"),
      EventBackground => GD.Load<Script>("res://addons/dialogic/Modules/Background/event_background.gd"),
      EventSound => GD.Load<Script>("res://addons/dialogic/Modules/Audio/event_sound.gd"),
      EventMusic => GD.Load<Script>("res://addons/dialogic/Modules/Audio/event_music.gd"),
      _ => throw new NotImplementedException("Unexpected event type!")
    };
    return resource;
  }



  //
  //  Records to parallel the built-in events, including those which serve little purpose at this moment

  // TODO: add documentation (maybe just rip from gd source??)
  // TODO: add default values from gd source. For easier use


  public abstract record Event() { // specifically not sealed to allow users creating custom event handlers
    public abstract void Apply(ref Resource resource);
  }

  public sealed record EventRaw(string RawTextCode) : Event {
    public override void Apply(ref Resource resource) { }
  }

  public sealed record EventText(string Text, Resource Character, string Portrait) : Event {
    public override void Apply(ref Resource resource) {
      resource.Set("text", Text);
      resource.Set("character", Character);
      resource.Set("portrait", Portrait);
    }
  }

  public sealed record EventWaitInput(bool HideTextBox) : Event {
    public override void Apply(ref Resource resource) => resource.Set("hide_textbox", HideTextBox);
  }

  public sealed record EventWait(float Time, bool HideText) : Event {
    public override void Apply(ref Resource resource) {
      resource.Set("time", Time);
      resource.Set("hide_text", HideText);
    }
  }

  public sealed record EventVoice(string FilePath, float Volume, string AudioBus) : Event {
    public override void Apply(ref Resource resource) {
      resource.Set("file_path", FilePath);
      resource.Set("volume", Volume);
      resource.Set("audio_bus", AudioBus);
    }
  }

  public sealed record EventVariable(string Name, EventVariable.Operations Operation, Variant Value, float RandomMin, float RandomMax) : Event {
    public override void Apply(ref Resource resource) {
      resource.Set("name", Name);
      resource.Set("operation", (int)Operation);
      resource.Set("value", Value);
      resource.Set("random_min", RandomMin);
      resource.Set("random_max", RandomMax);
    }

    public enum Operations { SET, ADD, SUBTRACT, MULTIPLY, DIVIDE };
  }
  public sealed record EventTextInput(string TextPrompt, string Variable, string Placeholder, string Default, bool AllowEmpty) : Event {
    public override void Apply(ref Resource resource) {
      resource.Set("text", TextPrompt);
      resource.Set("variable", Variable);
      resource.Set("placeholder", Placeholder);
      resource.Set("default", Default);
      resource.Set("allow_empty", AllowEmpty);
    }
  }

  public sealed record EventSignal(string Argument) : Event {
    public override void Apply(ref Resource resource) => resource.Set("argument", Argument);
  }

  public sealed record EventSetting(string Name, Variant Value, EventSetting.Modes Mode) : Event {
    public override void Apply(ref Resource resource) {
      resource.Set("name", Name);
      resource.Set("value", Value);
      resource.Set("mode", (int)Mode);
    }

    public enum Modes {

      SET, RESET, RESET_ALL
    }
  }
  public sealed record EventSave(string SaveSlot) : Event {
    public override void Apply(ref Resource resource) => resource.Set("slot_name", SaveSlot);
  }

  public sealed record EventReturn() : Event {
    public override void Apply(ref Resource resource) { }
  }

  public sealed record EventLabel(string Name) : Event {
    public override void Apply(ref Resource resource) => resource.Set("name", Name);
  }

  public sealed record EventJump(string LabelName, Resource TimelineTarget = null) : Event {
    public override void Apply(ref Resource resource) {
      resource.Set("timeline", TimelineTarget);
      resource.Set("label_name", LabelName);
    }
  }

  public sealed record EventHistory(EventHistory.Actions Action) : Event {
    public override void Apply(ref Resource resource) => resource.Set("action", (int)Action);

    public enum Actions { CLEAR, PAUSE, RESUME }
  }
  public sealed record EventGlossary() : Event // marked as "does nothing" in the source code :/
  {
    public override void Apply(ref Resource resource) { }
  }


  public sealed record EventEndBranch() : Event {
    public override void Apply(ref Resource resource) { }
  }

  public sealed record EventEnd() : Event {
    public override void Apply(ref Resource resource) { }
  }


  public sealed record EventCondition(EventCondition.ConditionType ConditionType_, string ConditionExpression) : Event {
    public override void Apply(ref Resource resource) {
      resource.Set("condition_type", (int)ConditionType_);
      resource.Set("condition", ConditionExpression);
    }

    public enum ConditionType { IF, ELIF, ELSE }
  }
  public sealed record EventComment(string Text) : Event // used in debug builds only
  {
    public override void Apply(ref Resource resource) => resource.Set("text", Text);
  }

  public sealed record EventChoice(string Text, string Condition, EventChoice.ElseActions ElseAction, string DisabledText) : Event {
    public override void Apply(ref Resource resource) {
      resource.Set("text", Text);
      resource.Set("condition", Condition);
      resource.Set("else_action", (int)ElseAction);
      resource.Set("disabled_text", DisabledText);
    }

    public enum ElseActions { HIDE, DISABLE, DEFAULT }

  }

  public sealed record EventPosition(EventPosition.Actions Action, int Position, Vector2 Vector, float MovementTime) : Event {
    public override void Apply(ref Resource resource) {
      resource.Set("action", (int)Action);
      resource.Set("position", Position);
      resource.Set("vector", Vector);
      resource.Set("movement_time", MovementTime);
    }

    public enum Actions {
      SET_RELATIVE, SET_ABSOLUTE, RESET, RESET_ALL
    }
  }

  public sealed record EventCharacter(EventCharacter.Actions Action = EventCharacter.Actions.JOIN, Resource Character = null, string Portrait = "", int Position = 1, string AnimationName = "", float AnimationLength = 0.5f, int AnimationRepeats = 1, bool AnimationWait = false, float PositionMoveTime = 0.0f, int ZIndex = 0, bool Mirror = false, string ExtraData = "") : Event {
    public override void Apply(ref Resource resource) {
      resource.Set("action", (int)Action);
      resource.Set("character", Character);
      resource.Set("portrait", Portrait);
      resource.Set("position", Position);
      resource.Set("animation_name", AnimationName);
      resource.Set("animation_length", AnimationLength);
      resource.Set("animation_repeats", AnimationRepeats);
      resource.Set("animation_wait", AnimationWait);
      resource.Set("position_move_time", PositionMoveTime);
      resource.Set("z_index", ZIndex);
      resource.Set("mirrored", Mirror);
      resource.Set("extra_data", ExtraData);
    }

    public enum Actions { JOIN, LEAVE, UPDATE }
  }
  public sealed record EventCallNode(string Path, string Method, Godot.Collections.Array Args, bool Wait, bool Inline, string InlineSignalArgument, bool InlineSignalUse) : Event {
    public override void Apply(ref Resource resource) {
      resource.Set("path", Path);
      resource.Set("method", Method);
      resource.Set("arguments", Args);
      resource.Set("wait", Wait);
      resource.Set("inline", Inline);
      resource.Set("inline_signal_argument", InlineSignalArgument);
      resource.Set("inline_signal_use", InlineSignalUse);
    }
  }


  public sealed record EventBackground(string Scene, string Argument, float Fade) : Event {
    public override void Apply(ref Resource resource) {
      resource.Set("scene", Scene);
      resource.Set("argument", Argument);
      resource.Set("fade", Fade);
    }
  }


  public sealed record EventSound(string FilePath, float Volume, string AudioBus, bool Loop) : Event {
    public override void Apply(ref Resource resource) {
      resource.Set("file_path", FilePath);
      resource.Set("volume", Volume);
      resource.Set("audio_bus", AudioBus);
      resource.Set("loop", Loop);
    }
  }

  public sealed record EventMusic(string FilePath, float FadeLength, float Volume, string AudioBus, bool Loop) : Event {
    public override void Apply(ref Resource resource) {
      resource.Set("file_path", FilePath);
      resource.Set("fade_length", FadeLength);
      resource.Set("volume", Volume);
      resource.Set("audio_bus", AudioBus);
      resource.Set("loop", Loop);
    }
  }
}


