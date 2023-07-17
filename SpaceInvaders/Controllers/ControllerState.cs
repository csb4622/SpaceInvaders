using Microsoft.Xna.Framework;

namespace SpaceInvaders.Controllers;

public class ControllerState
{
    public Point DeltaPosition { get; }
    public bool Shoot { get; }

    public ControllerState(Point deltaPosition, bool shoot)
    {
        DeltaPosition = deltaPosition;
        Shoot = shoot;
    }
}