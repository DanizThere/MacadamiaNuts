using UnityEngine;

namespace MacadamiaNuts.Golden
{
    public class GoldenHead : MonoBehaviour
    {
        [SerializeField] private Sound _corryptSound;
        [SerializeField] private Sound _startCorryptSound;

        private GoldenPlayerAvatar _goldenAvatar;
        private GoldenVingette _goldenVingette;

        public void Initialize()
        {
            _goldenAvatar = GetComponent<GoldenPlayerAvatar>();
            _goldenVingette = GetComponent<GoldenVingette>();
        }

        public void IncreaseCorryption()
        {
            _goldenAvatar.IncreaseCorryption();
            _goldenVingette.ShowCurrentVignette(_goldenAvatar.Counter, _goldenAvatar.MaxCorruption);
            _corryptSound.Play(transform.position);
        }

        public void ShowVingette()
        {
            _goldenVingette.Show();
            _goldenVingette.ShowCurrentVignette(.7f, _goldenAvatar.MaxCorruption);
        }

        public void HideVingette()
        {
            _goldenVingette.Hide();
        }

        public void ResetVingette()
        {
            _goldenVingette.ResetVingette();
        }

        public void Revive()
        {
            Destroy(this);
        }

        public void Kill()
        {
            ResetVingette();
        }

        public void StartCorrypt()
        {
            _startCorryptSound.Play(transform.position);

            ShowVingette();

            if (SemiFunc.IsMultiplayer())
            {
                var message = MacadamiaPhrasesDictionary.Instance.GetGoldenNutsCorryptionPhrase();

                ChatManager.instance.PossessChatScheduleStart(15);
                ChatManager.instance.PossessChat(ChatManager.PossessChatID.None, message, typingSpeed: .1f, possessColor);
                ChatManager.instance.PossessChatScheduleEnd();
            }
        }
    }
}
