namespace Architect.Behaviour.Utility;

public class CustomHazardRespawnMarker : HazardRespawnMarker
{
    private void Start()
    {
        respawnFacingRight = transform.GetScaleX() < 0;
    }
}