using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace DesignPatternCatalog.Services.PatternImplementations;

// Design Pattern: Observer
// Code Components: IObserver, UserSubscriber, PushGatewayObserver, DiscordWebhookObserver, EmailDigestObserver, TelegramBotObserver, ISubject, YouTubeChannel
// Purpose: Automatically broadcasts instant multi-channel push, email, and chat notifications to all registered subscriber listeners when publisher events occur.

public interface IObserver
{
    string ObserverType { get; }
    string Notify(string videoTitle, string channelName, string eventPriority);
}

public class UserSubscriber : IObserver
{
    public string UserName { get; }
    public string ObserverType => "User Mobile Subscriber";

    public UserSubscriber(string name) { UserName = name; }

    public string Notify(string videoTitle, string channelName, string eventPriority) =>
        $"  • [USER CLIENT] '{UserName}' received instant alert: [{eventPriority}] \"{videoTitle}\"";
}

public class PushGatewayObserver : IObserver
{
    public string ObserverType => "Apple APNs / Google FCM Push Gateway";

    public string Notify(string videoTitle, string channelName, string eventPriority) =>
        $"  • [PUSH SERVICE] APNs/FCM dispatched 128,450 mobile device push notifications for \"{videoTitle}\".";
}

public class DiscordWebhookObserver : IObserver
{
    public string ObserverType => "Discord Community Webhook Bot";

    public string Notify(string videoTitle, string channelName, string eventPriority) =>
        $"  • [DISCORD BOT] Posted rich embed card to #announcements: [{eventPriority}] \"{videoTitle}\".";
}

public class EmailDigestObserver : IObserver
{
    public string ObserverType => "Email Newsletter Digest Service";

    public string Notify(string videoTitle, string channelName, string eventPriority) =>
        $"  • [EMAIL QUEUE] Queued personalized email digest for 42,900 newsletter subscribers.";
}

public class TelegramBotObserver : IObserver
{
    public string ObserverType => "Telegram VIP Broadcast Bot";

    public string Notify(string videoTitle, string channelName, string eventPriority) =>
        $"  • [TELEGRAM VIP] Broadcasted pinned announcement to 15,200 channel members.";
}

public interface ISubject
{
    void Subscribe(IObserver observer);
    void Unsubscribe(IObserver observer);
    List<string> Broadcast(string videoTitle, string priority);
}

public class YouTubeChannel : ISubject
{
    public string ChannelName { get; }
    private readonly List<IObserver> _subscribers = new();

    public YouTubeChannel(string channelName)
    {
        ChannelName = channelName;
    }

    public void Subscribe(IObserver observer) => _subscribers.Add(observer);
    public void Unsubscribe(IObserver observer) => _subscribers.Remove(observer);

    public List<string> Broadcast(string videoTitle, string priority)
    {
        var logs = new List<string>();
        foreach (var sub in _subscribers)
        {
            logs.Add(sub.Notify(videoTitle, ChannelName, priority));
        }
        return logs;
    }
}

// Design Pattern: Strategy
// Code Components: RouteResult, IRouteStrategy, HighwayExpressStrategy, BicycleScenicStrategy, PublicTransitStrategy, EvEcoStrategy, NavigatorContext
// Purpose: Encapsulates interchangeable route navigation algorithms (express highway, bike trail, transit, EV) to calculate travel time, fare, and carbon metrics dynamically.

public class RouteResult
{
    public string StrategyName { get; set; } = string.Empty;
    public string RouteDescription { get; set; } = string.Empty;
    public double BaseDistanceKm { get; set; }
    public double CalculatedDurationHours { get; set; }
    public double TollAndFareCost { get; set; }
    public double CarbonEmissionKg { get; set; }
}

public interface IRouteStrategy
{
    RouteResult CalculateRoute(string origin, string destination, double trafficMultiplier, HashSet<string> preferenceToggles);
}

public class HighwayExpressStrategy : IRouteStrategy
{
    public RouteResult CalculateRoute(string origin, string destination, double trafficMultiplier, HashSet<string> preferenceToggles)
    {
        bool avoidTolls = preferenceToggles.Any(t => t.Contains("Avoid Highway Tolls"));
        bool hovLane = preferenceToggles.Any(t => t.Contains("Carpool"));

        double distance = avoidTolls ? 168.0 : 148.0;
        double speed = (avoidTolls ? 55.0 : 75.0) / (hovLane ? Math.Min(trafficMultiplier, 1.1) : trafficMultiplier);
        double duration = distance / speed;
        double tollCost = avoidTolls ? 0.0 : 340.0;
        double carbon = distance * 0.125;

        return new RouteResult
        {
            StrategyName = "Highway Express Driving Strategy",
            RouteDescription = avoidTolls ? "State Highway 42 Corridor (Toll-Free Route)" : "National Express 6-Lane Expressway (Fastest)",
            BaseDistanceKm = distance,
            CalculatedDurationHours = duration,
            TollAndFareCost = tollCost,
            CarbonEmissionKg = carbon
        };
    }
}

public class BicycleScenicStrategy : IRouteStrategy
{
    public RouteResult CalculateRoute(string origin, string destination, double trafficMultiplier, HashSet<string> preferenceToggles)
    {
        double distance = 154.0;
        double speed = 18.0;
        double duration = distance / speed;

        return new RouteResult
        {
            StrategyName = "Scenic Greenway Bicycle Strategy",
            RouteDescription = "Western Scenic Bicycle Trail & Dedicated Riverway",
            BaseDistanceKm = distance,
            CalculatedDurationHours = duration,
            TollAndFareCost = 0.0,
            CarbonEmissionKg = 0.0
        };
    }
}

public class PublicTransitStrategy : IRouteStrategy
{
    public RouteResult CalculateRoute(string origin, string destination, double trafficMultiplier, HashSet<string> preferenceToggles)
    {
        double distance = 152.0;
        double duration = 2.75;
        double fare = 480.0;
        double carbon = distance * 0.035;

        return new RouteResult
        {
            StrategyName = "Metropolitan Transit (Metro + Vande Bharat Express)",
            RouteDescription = "Metro Line 3 -> High-Speed Vande Bharat Intercity Rail",
            BaseDistanceKm = distance,
            CalculatedDurationHours = duration,
            TollAndFareCost = fare,
            CarbonEmissionKg = carbon
        };
    }
}

public class EvEcoStrategy : IRouteStrategy
{
    public RouteResult CalculateRoute(string origin, string destination, double trafficMultiplier, HashSet<string> preferenceToggles)
    {
        bool includeChargers = preferenceToggles.Any(t => t.Contains("EV Fast Charging"));
        double distance = 150.0;
        double speed = 68.0 / trafficMultiplier;
        double duration = (distance / speed) + (includeChargers ? 0.4 : 0.0);
        double powerCost = 180.0 + (includeChargers ? 120.0 : 0.0);

        return new RouteResult
        {
            StrategyName = "Eco-Optimized Electric Vehicle (EV) Route",
            RouteDescription = "Green Corridor EV Expressway with High-Power DC Fast Chargers",
            BaseDistanceKm = distance,
            CalculatedDurationHours = duration,
            TollAndFareCost = powerCost,
            CarbonEmissionKg = 0.0
        };
    }
}

public class NavigatorContext
{
    private IRouteStrategy _strategy;

    public NavigatorContext(IRouteStrategy initialStrategy)
    {
        _strategy = initialStrategy;
    }

    public void SetStrategy(IRouteStrategy strategy) => _strategy = strategy;

    public string Calculate(string origin, string destination, double trafficMultiplier, HashSet<string> toggles)
    {
        RouteResult r = _strategy.CalculateRoute(origin, destination, trafficMultiplier, toggles);
        int hours = (int)r.CalculatedDurationHours;
        int minutes = (int)Math.Round((r.CalculatedDurationHours - hours) * 60);

        bool showCarbon = toggles.Any(t => t.Contains("Carbon"));

        return $"[STRATEGY PATTERN DYNAMIC ROUTE COMPUTATION]\n" +
               $"------------------------------------------------------------\n" +
               $"• Active Strategy:      {r.StrategyName}\n" +
               $"• Trip Itinerary:       \"{origin}\" ➔ \"{destination}\"\n" +
               $"• Route Path:           {r.RouteDescription}\n" +
               $"• Total Distance:       {r.BaseDistanceKm:F1} km\n" +
               $"• Estimated Trip Time:  {hours}h {minutes}m (Traffic Multiplier: {trafficMultiplier:F2}x)\n" +
               $"• Estimated Toll/Fare:  ₹{r.TollAndFareCost:F2}\n" +
               (showCarbon ? $"• Carbon Emission:      {r.CarbonEmissionKg:F1} kg CO2\n" : "") +
               $"------------------------------------------------------------\n" +
               $"STATUS: Algorithm executed dynamically via Navigator Strategy Context.";
    }
}

// Design Pattern: Command
// Code Components: ICommand, TextEditor, InsertTextCommand, TransformCaseCommand, CommandHistory
// Purpose: Encapsulates editor operations as discrete command objects with full execution and reverse Undo capabilities stored on a history stack.

public interface ICommand
{
    string CommandName { get; }
    void Execute();
    void Undo();
}

public class TextEditor
{
    public string Buffer { get; set; } = string.Empty;
}

public class InsertTextCommand : ICommand
{
    private readonly TextEditor _editor;
    private readonly string _text;
    private string _previousState = string.Empty;

    public string CommandName => $"InsertTextCommand(\"{_text}\")";

    public InsertTextCommand(TextEditor editor, string text)
    {
        _editor = editor;
        _text = text;
    }

    public void Execute()
    {
        _previousState = _editor.Buffer;
        _editor.Buffer = string.IsNullOrEmpty(_editor.Buffer) ? _text : $"{_editor.Buffer} {_text}";
    }

    public void Undo()
    {
        _editor.Buffer = _previousState;
    }
}

public class TransformCaseCommand : ICommand
{
    private readonly TextEditor _editor;
    private readonly string _mode;
    private string _previousState = string.Empty;

    public string CommandName => $"TransformCaseCommand({_mode})";

    public TransformCaseCommand(TextEditor editor, string mode)
    {
        _editor = editor;
        _mode = mode;
    }

    public void Execute()
    {
        _previousState = _editor.Buffer;
        if (_mode.Contains("UPPERCASE"))
            _editor.Buffer = _editor.Buffer.ToUpperInvariant();
        else if (_mode.Contains("Title"))
            _editor.Buffer = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(_editor.Buffer.ToLowerInvariant());
        else if (_mode.Contains("Markdown Quotes"))
            _editor.Buffer = $"> {_editor.Buffer}";
        else if (_mode.Contains("Clear"))
            _editor.Buffer = string.Empty;
    }

    public void Undo()
    {
        _editor.Buffer = _previousState;
    }
}

public class CommandHistory
{
    private readonly Stack<ICommand> _history = new();

    public void Push(ICommand cmd) => _history.Push(cmd);
    public ICommand? Pop() => _history.Count > 0 ? _history.Pop() : null;
    public int Count => _history.Count;
    public IEnumerable<string> GetHistoryList() => _history.Select(c => c.CommandName);
}

// Design Pattern: State
// Code Components: IAudioState, StoppedState, PlayingState, PausedState, LockedState, AudioPlayerContext
// Purpose: Controls media playback lifecycle and button interactions by dynamically transitioning the active state object (stopped, playing, paused, locked).

public interface IAudioState
{
    string StateName { get; }
    string ClickPlay(AudioPlayerContext context, string trackName, int bitrate, string eqProfile);
    string ClickPause(AudioPlayerContext context);
    string ClickLock(AudioPlayerContext context);
}

public class StoppedState : IAudioState
{
    public string StateName => "Stopped State";

    public string ClickPlay(AudioPlayerContext context, string trackName, int bitrate, string eqProfile)
    {
        context.State = new PlayingState();
        return $"Started Streaming \"{trackName}\" at {bitrate} kbps Hi-Res | EQ: [{eqProfile}].";
    }

    public string ClickPause(AudioPlayerContext context) => "Action Ignored: Player is currently stopped.";

    public string ClickLock(AudioPlayerContext context)
    {
        context.State = new LockedState(this);
        return "Controls Locked while stopped.";
    }
}

public class PlayingState : IAudioState
{
    public string StateName => "Playing State";

    public string ClickPlay(AudioPlayerContext context, string trackName, int bitrate, string eqProfile) =>
        $"Track \"{trackName}\" is already playing actively.";

    public string ClickPause(AudioPlayerContext context)
    {
        context.State = new PausedState();
        return "Playback Paused: Audio buffer frozen at current playback sample.";
    }

    public string ClickLock(AudioPlayerContext context)
    {
        context.State = new LockedState(this);
        return "Controls Locked while music continues streaming in background.";
    }
}

public class PausedState : IAudioState
{
    public string StateName => "Paused State";

    public string ClickPlay(AudioPlayerContext context, string trackName, int bitrate, string eqProfile)
    {
        context.State = new PlayingState();
        return $"Playback Resumed: Streaming \"{trackName}\" ({bitrate} kbps).";
    }

    public string ClickPause(AudioPlayerContext context) => "Already paused.";

    public string ClickLock(AudioPlayerContext context)
    {
        context.State = new LockedState(this);
        return "Controls Locked in paused mode.";
    }
}

public class LockedState : IAudioState
{
    private readonly IAudioState _previousState;
    public string StateName => "Locked State";

    public LockedState(IAudioState previous) { _previousState = previous; }

    public string ClickPlay(AudioPlayerContext context, string trackName, int bitrate, string eqProfile) =>
        "Touch Controls Locked: Unlock device before interacting.";

    public string ClickPause(AudioPlayerContext context) =>
        "Touch Controls Locked: Unlock device before interacting.";

    public string ClickLock(AudioPlayerContext context)
    {
        context.State = _previousState;
        return $"Controls Unlocked -> Restored to [{_previousState.StateName}].";
    }
}

public class AudioPlayerContext
{
    public IAudioState State { get; set; } = new StoppedState();
}
