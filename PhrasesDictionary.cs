using UnityEngine;

namespace MacadamiaNuts
{
    public class MacadamiaPhrasesDictionary : MonoBehaviour
    {
        public static MacadamiaPhrasesDictionary Instance;

        private string[] _macadamiaNutsEatPhrase;
        private string[] _goldenNutsStartCorryption;
        private string[] _macadamiaNutsSwear;
        private string[] _swearEmoji;
        private string[] _dizzyEmoji;

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
