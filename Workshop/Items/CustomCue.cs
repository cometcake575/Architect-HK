using Architect.Behaviour.Utility;
using Architect.Storage;
using UnityEngine;

namespace Architect.Workshop.Items;

public class CustomCue : WorkshopItem
{
    private static readonly Sprite Icon = ResourceUtils.LoadSpriteResource("audio_player");
    
    private MusicCue _mcue;
    private MusicCue.MusicChannelInfo _mci;
    
    public string WavUrl = string.Empty;
    
    public override (string, string)[] FilesToDownload => [(WavUrl, "wav")];

    public override void Register()
    {
        _mcue = ScriptableObject.CreateInstance<MusicCue>();

        _mci = new MusicCue.MusicChannelInfo();

        _mcue.alternatives = [];
        _mcue.channelInfos =
        [
            _mci,
            new MusicCue.MusicChannelInfo(),
            new MusicCue.MusicChannelInfo(),
            new MusicCue.MusicChannelInfo(),
            new MusicCue.MusicChannelInfo(),
            new MusicCue.MusicChannelInfo()
        ];
        _mcue.name = Id;
        _mcue.originalMusicEventName = string.Empty;

        AudioPlayer.MusicCues.Add(Id, _mcue);

        RefreshSound();
    }

    private void RefreshSound()
    {
        if (WavUrl.IsNullOrWhiteSpace()) return;
        CustomAssetManager.DoLoadSound(WavUrl, wav =>
        {
            wav.LoadAudioData();
            _mci.clip = wav;
        });
    }
    
    public override void Unregister()
    {
        if (_mcue) Object.Destroy(_mcue);
        AudioPlayer.MusicCues.Remove(Id);
        AudioPlayer.AtmosCues.Remove(Id);
    }

    public override Sprite GetIcon()
    {
        return Icon;
    }
}