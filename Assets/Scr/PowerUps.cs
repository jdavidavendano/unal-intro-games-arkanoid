using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUps : MonoBehaviour
{
    public float speed;
    public int _type;
    /*
        types:
            1: slow
            2: fast
            3: multiple balls
            4: small
            5: large
    */
    private SpriteRenderer _renderer;
    private const string BLOCK_BIG_PATH = "Sprites/PowerUpsTiles/powerUp_{0}";


    public void SetData(int type)
    {
        _type = type;
    }
    public void Init()
    {
        _renderer = GetComponentInChildren<SpriteRenderer>();
        _renderer.sprite = GetPowerUpSprite(_type);
    }


    void Update()
    {
        transform.Translate(new Vector2(0f, -0.5f) * Time.deltaTime * speed);
    }

    static Sprite GetPowerUpSprite(int type)
    {
        string path = string.Empty;

        path = string.Format(BLOCK_BIG_PATH, type);

        return Resources.Load<Sprite>(path);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
