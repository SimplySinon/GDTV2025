

using System;
using UnityEngine.Events;


public class Helpers
{
    // Used along side an EventChannelInstance to raise an event
    public static void RaiseIfNotNull<T>(GenericEventChannelSO<T> channel, T param)
    {
        if (channel != null)
        {
            channel.RaiseEvent(param);
        }
    }

    // Used along side an EventChannelInstance to subscribe to an event
    public static void SubscribeIfNotNull<T>(GenericEventChannelSO<T> channel, UnityAction<T> func)
    {
        if (channel != null)
        {
            channel.OnEventRaised += func;
        }
    }

    // Used along side an EventChannelInstance to unsubscribe from an event
    public static void UnsubscribeIfNotNull<T>(GenericEventChannelSO<T> channel, UnityAction<T> func)
    {
        if (channel != null)
        {
            channel.OnEventRaised -= func;
        }
    }
}

