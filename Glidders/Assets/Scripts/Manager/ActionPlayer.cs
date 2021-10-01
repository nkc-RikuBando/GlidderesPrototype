using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ActionPlayer
{
    private static IActable actable;

    public static void Bind(this IActable act)
    {
        actable = act;
    }

    public static void Play()
    {
        actable.ActionPlaying();
    }
}
