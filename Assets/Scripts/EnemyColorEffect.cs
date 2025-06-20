using UnityEngine;

public class EnemyColorEffect : MonoBehaviour
{
    public Material whiteMaterial; // Material branco
    private Material defaultMaterial;
    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        defaultMaterial = sr.material;
    }

    // Deixa o inimigo branco
    public void SetWhite()
    {
        sr.material = whiteMaterial;
    }

    // Retorna ao material padrão
    public void ResetColor()
    {
        sr.material = defaultMaterial;
    }

    // Faz piscar branco rapidamente (efeito de dano, por exemplo)
    public void FlashWhite(float duration = 0.1f)
    {
        SetWhite();
        Invoke(nameof(ResetColor), duration);
    }
}
