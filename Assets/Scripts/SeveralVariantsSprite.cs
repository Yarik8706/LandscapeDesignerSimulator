using UnityEngine;
using Random = System.Random;

namespace RoomObjects
{
    public class SeveralVariantsSprite : MonoBehaviour
    {
        public Sprite[] sprites;
        public GameObject[] decorations;
        public bool flipToX;
        public bool flipToY;

        private SpriteRenderer _spriteRenderer;
        private GameObject _decoration;
        
        // Start is called before the first frame update
        private void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            var randomNumber = new Random();
            if (sprites.Length > 1)
            {
                _spriteRenderer.sprite = sprites[randomNumber.Next(0, sprites.Length)];
            }
            if (flipToX && randomNumber.Next(0, 2) == 0)
            {
                _spriteRenderer.flipX = true;
            }
            if (flipToY && randomNumber.Next(0, 2) == 0)
            {
                _spriteRenderer.flipY = true;
            }

            if(decorations.Length == 0) return;
            var randomNumberForDecoration = randomNumber.Next(0, decorations.Length);
            if (decorations[randomNumberForDecoration] == null) return;
            _decoration = Instantiate(decorations[randomNumberForDecoration], transform.position, Quaternion.identity);
            _decoration.transform.SetParent(transform);
        }
    }
}
