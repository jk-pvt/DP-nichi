using System;
using System.Collections.Generic;

namespace DesignPatternCatalog.Services.PatternImplementations;

public interface IObserver
{
    string Notify(string videoTitle, string channelName);
}

public class UserSubscriber : IObserver
{
    public string UserName { get; }
    public UserSubscriber(string name) { UserName = name; }
    public string Notify(string videoTitle, string channelName) =>
        $"• [USER SUB] User '{UserName}' (Mobile Client) -> Received instant push alert: \"New Video: {videoTitle}\"";
}

public class PushGatewayObserver : IObserver
{
    public string Notify(string videoTitle, string channelName) =>
        $"• [SYSTEM] APNs/FCM Push Gateway -> Dispatched 14,820 mobile notification tokens for '{videoTitle}'.";
}

public class DiscordWebhookObserver : IObserver
{
    public string Notify(string videoTitle, string channelName) =>
        $"• [WEBHOOK] Discord Community Bot -> Posted embed banner to #announcements.";
}

public class EmailDigestObserver : IObserver
{
    public string Notify(string videoTitle, string channelName) =>
        $"• [EMAIL] Email Dispatcher -> Queued weekly digest for 5,200 subscribers.";
}

public interface ISubject
{
    void Subscribe(IObserver observer);
    void Unsubscribe(IObserver observer);
    List<string> UploadVideo(string videoTitle);
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

    public List<string> UploadVideo(string videoTitle)
    {
        var logs = new List<string>();
        foreach (var sub in _subscribers)
        {
            logs.Add(sub.Notify(videoTitle, ChannelName));
        }
        return logs;
    }
}

public class RouteResult
{
    public string StrategyName { get; set; } = string.Empty;
    public string Corridors { get; set; } = string.Empty;
    public double DistanceKm { get; set; }
    public string EstimatedTime { get; set; } = string.Empty;
    public double TollCost { get; set; }
    public double CarbonKg { get; set; }
}

public interface IRouteStrategy
{
    RouteResult BuildRoute(string origin, string destination);
}

public class HighwayExpressStrategy : IRouteStrategy
{
    public RouteResult BuildRoute(string origin, string destination) => new()
    {
        StrategyName = "RoadHighwayStrategy",
        Corridors = "National Express 6-Lane Corridor",
        DistanceKm = 148.0,
        EstimatedTime = "2 hours 15 mins (Avg Speed: 65.7 km/h)",
        TollCost = 320.00,
        CarbonKg = 18.4
    };
}

public class BicycleScenicStrategy : IRouteStrategy
{
    public RouteResult BuildRoute(string origin, string destination) => new()
    {
        StrategyName = "BicycleScenicStrategy",
        Corridors = "Western Scenic Greenway Route",
        DistanceKm = 156.0,
        EstimatedTime = "8 hours 30 mins",
        TollCost = 0.00,
        CarbonKg = 0.0
    };
}

public class PublicTransitStrategy : IRouteStrategy
{
    public RouteResult BuildRoute(string origin, string destination) => new()
    {
        StrategyName = "PublicTransitStrategy",
        Corridors = "Express Train & Metro Link",
        DistanceKm = 152.0,
        EstimatedTime = "3 hours 05 mins",
        TollCost = 560.00,
        CarbonKg = 4.2
    };
}

public class NavigatorContext
{
    private IRouteStrategy _strategy;

    public NavigatorContext(IRouteStrategy initialStrategy)
    {
        _strategy = initialStrategy;
    }

    public void SetStrategy(IRouteStrategy strategy) => _strategy = strategy;

    public string Calculate(string origin, string destination)
    {
        RouteResult r = _strategy.BuildRoute(origin, destination);
        return $"[STRATEGY PATTERN: {r.StrategyName}]\n" +
               $"------------------------------------------------------------\n" +
               $"• Active Strategy:     {_strategy.GetType().Name} (implements IRouteStrategy)\n" +
               $"• Route Direction:     From \"{origin}\" To \"{destination}\"\n" +
               $"• Algorithm Corridor:  {r.Corridors}\n" +
               $"• Distance:            {r.DistanceKm:F1} km\n" +
               $"• Estimated Time:      {r.EstimatedTime}\n" +
               $"• Toll/Fare Cost:      ₹{r.TollCost:F2}\n" +
               $"• Carbon Footprint:    {r.CarbonKg:F1} kg CO2\n" +
               $"------------------------------------------------------------\n" +
               $"STATUS: Algorithm executed dynamically via Navigator Context.";
    }
}

public interface ICommand
{
    void Execute();
    void Undo();
    string GetName();
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

    public InsertTextCommand(TextEditor editor, string text)
    {
        _editor = editor;
        _text = text;
    }

    public void Execute()
    {
        _previousState = _editor.Buffer;
        _editor.Buffer += _text;
    }

    public void Undo()
    {
        _editor.Buffer = _previousState;
    }

    public string GetName() => $"InsertTextCommand(\"{_text}\")";
}

public class ChangeCaseCommand : ICommand
{
    private readonly TextEditor _editor;
    private string _previousState = string.Empty;

    public ChangeCaseCommand(TextEditor editor)
    {
        _editor = editor;
    }

    public void Execute()
    {
        _previousState = _editor.Buffer;
        _editor.Buffer = _editor.Buffer.ToUpperInvariant();
    }

    public void Undo()
    {
        _editor.Buffer = _previousState;
    }

    public string GetName() => "ChangeCaseCommand(UPPERCASE)";
}

public class CommandHistory
{
    private readonly Stack<ICommand> _history = new();

    public void Push(ICommand cmd) => _history.Push(cmd);
    public ICommand? Pop() => _history.Count > 0 ? _history.Pop() : null;
    public int Count => _history.Count;
}

public interface IAudioState
{
    string ClickPlay(AudioPlayerContext context);
    string ClickPause(AudioPlayerContext context);
    string ClickLock(AudioPlayerContext context);
    string StateName { get; }
}

public class StoppedState : IAudioState
{
    public string StateName => "StoppedState";
    public string ClickPlay(AudioPlayerContext context)
    {
        context.State = new PlayingState();
        return "Transitioned -> PlayingState | Decoding 320kbps audio stream.";
    }
    public string ClickPause(AudioPlayerContext context) => "Ignored: Cannot pause when stopped.";
    public string ClickLock(AudioPlayerContext context)
    {
        context.State = new LockedState(this);
        return "Transitioned -> LockedState | Player locked in stopped mode.";
    }
}

public class PlayingState : IAudioState
{
    public string StateName => "PlayingState";
    public string ClickPlay(AudioPlayerContext context) => "Track already playing.";
    public string ClickPause(AudioPlayerContext context)
    {
        context.State = new PausedState();
        return "Transitioned -> PausedState | Audio buffer frozen at current sample.";
    }
    public string ClickLock(AudioPlayerContext context)
    {
        context.State = new LockedState(this);
        return "Transitioned -> LockedState | Player locked in playback mode.";
    }
}

public class PausedState : IAudioState
{
    public string StateName => "PausedState";
    public string ClickPlay(AudioPlayerContext context)
    {
        context.State = new PlayingState();
        return "Transitioned -> PlayingState | Resumed audio stream.";
    }
    public string ClickPause(AudioPlayerContext context) => "Already paused.";
    public string ClickLock(AudioPlayerContext context)
    {
        context.State = new LockedState(this);
        return "Transitioned -> LockedState | Player locked in paused mode.";
    }
}

public class LockedState : IAudioState
{
    private readonly IAudioState _previousState;
    public string StateName => "LockedState";
    public LockedState(IAudioState previous) { _previousState = previous; }

    public string ClickPlay(AudioPlayerContext context) => "Controls are locked. Click Unlock.";
    public string ClickPause(AudioPlayerContext context) => "Controls are locked. Click Unlock.";
    public string ClickLock(AudioPlayerContext context)
    {
        context.State = _previousState;
        return $"Transitioned -> {_previousState.StateName} | Player controls unlocked.";
    }
}

public class AudioPlayerContext
{
    public IAudioState State { get; set; } = new StoppedState();
}
