using UnityEngine;

namespace MacadamiaNuts.Golden
{
    public class GoldenHead : MonoBehaviour
    {
        public float PlayerCost => _playerCost;

#pragma warning disable CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Рассмотрите возможность добавления модификатора "required" или объявления значения, допускающего значение NULL.
        private Sound _corryptSound;
        private Sound _startCorryptSound;

        private GoldenPlayerAvatar _goldenAvatar;
        private GoldenVingette _goldenVingette;
        private PlayerAvatar _playerAvatar;
#pragma warning restore CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Рассмотрите возможность добавления модификатора "required" или объявления значения, допускающего значение NULL.

        private bool _playerIsGrabbing;
        private float _playerCost;


        private void Awake() 
        {
            _goldenAvatar = GetComponent<GoldenPlayerAvatar>();
            _goldenVingette = GetComponent<GoldenVingette>();
        }

        public void Initialize(PlayerAvatar playerAvatar, Sound corryptSound, Sound startCorryptSound)
        {
            _playerAvatar = playerAvatar;
            _corryptSound = corryptSound;
            _startCorryptSound = startCorryptSound;
            StartCorrypt();
        }

        public void IncreaseCorryption()
        {
            if (_playerIsGrabbing) return;

            _corryptSound.Play(_playerAvatar.spectatePoint.position);

            _goldenAvatar.IncreaseCorryption();
            _goldenVingette.ShowCurrentVignette(_goldenAvatar.Counter, _goldenAvatar.MaxCorruption);
            _playerIsGrabbing = true;
        }

        public void ResetGrabbing()
        {
            _playerIsGrabbing = false;
        }

        public void ShowVingette()
        {
            _goldenVingette.Show();
            _goldenVingette.ShowCurrentVignette(1f, _goldenAvatar.MaxCorruption);
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
            Destroy(gameObject);
        }

        public void Kill()
        {
            _playerCost = _goldenAvatar.CostValue;
            ResetVingette();
        }

        public void StartCorrypt()
        {
            _startCorryptSound.Play(_playerAvatar.spectatePoint.position);

            ShowVingette();

            if (SemiFunc.IsMultiplayer())
            {
                var message = MacadamiaPhrasesDictionary.Instance.GetGoldenNutsCorryptionPhrase();

                Color possessColor = Color.yellow;
                ChatManager.instance.PossessChatScheduleStart(15);
                ChatManager.instance.PossessChat(ChatManager.PossessChatID.None, message, typingSpeed: 5f, possessColor);
                ChatManager.instance.PossessChatScheduleEnd();
            }
        }
    }
}
