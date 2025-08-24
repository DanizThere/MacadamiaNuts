using UnityEngine;

namespace MacadamiaNuts
{
    public class MacadamiaPhrasesDictionary : MonoBehaviour
    {
#pragma warning disable CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Рассмотрите возможность добавления модификатора "required" или объявления значения, допускающего значение NULL.
        public static MacadamiaPhrasesDictionary Instance;

        private string[] _macadamiaNutsEatPhrase;
        private string[] _goldenNutsStartCorryption;
        private string[] _macadamiaNutsSwear;
        private string[] _swearEmoji;
        private string[] _dizzyEmoji;
#pragma warning restore CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Рассмотрите возможность добавления модификатора "required" или объявления значения, допускающего значение NULL.

        public void Initialize()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            InitializeEatPhrases();
            InitializeCorryptionPhrases();
            InitializeSwearWords();
            InitializeSwearEmoji();
            InitializeDizzyEmoji();
        }

        public string GetMacadamiaNutsEatPhrase()
        {
            return _macadamiaNutsEatPhrase[Random.Range(0, _macadamiaNutsEatPhrase.Length)];
        }

        public string GetGoldenNutsCorryptionPhrase()
        {
            return $"{_goldenNutsStartCorryption[Random.Range(0, _goldenNutsStartCorryption.Length)]}, {_dizzyEmoji[Random.Range(0, _dizzyEmoji.Length)]}";
        }

        public string GetMacadamiaSwearPhrase()
        {
            return $"{_macadamiaNutsSwear[Random.Range(0, _macadamiaNutsSwear.Length)]}, {_swearEmoji[Random.Range(0, _swearEmoji.Length)]}";
        }


        private void InitializeEatPhrases()
        {
            _macadamiaNutsEatPhrase = new string[] { };
        }

        private void InitializeCorryptionPhrases()
        {
            _goldenNutsStartCorryption = new string[] { "Oh, this is a very creepy feeling" };
        }

        private void InitializeSwearWords()
        {
            _macadamiaNutsSwear = new string[] { };
        }

        private void InitializeSwearEmoji()
        {
            _swearEmoji = new string[] { };
        }

        private void InitializeDizzyEmoji()
        {
            _dizzyEmoji = new string[] { ">//<" };
        }
    }
}
