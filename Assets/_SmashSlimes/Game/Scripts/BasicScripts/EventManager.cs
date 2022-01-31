using System;
using UniRx;

namespace SmashSlimes
{
    public enum StateEvent
    {
        GameStart,
        Complete,
        Failed,
        Continue,
        GameEnd,
        Pose,
        ReStart
    }

    public enum HitColliderType
    {
        Great,
        Good,
        Bad,
        AssistUi
    }

    public enum HitColliderObjectType
    {
        Great,
        Good,
        Bad,
        Root
    }

    public enum SlimeType
    {
        Normal,
        Metal,
        Fast
    }

    public enum HitResultType
    {
        Great,
        Good,
        Bad1,
        Bad2
    }

    public enum GroundType
    {
        Center,
        Left,
        Right
    }

    public enum MyObjectId
    {
        Id0,
        Id1,
        Id2,
        Id3,
        Id4,
        Id5,
        Id6,
        Id7,
        Id8,
        Id9
    }
    public static class EventManager
    {
        public static IObservable<StateEvent> OnReceiveAsObservable(StateEvent stateEvent)
        {
            return MessageBroker.Default.Receive<StateEvent>().Where(ge => ge == stateEvent);
        }

        public static void Send(StateEvent stateEvent)
        {
            MessageBroker.Default.Publish(stateEvent);
        }
    }
}