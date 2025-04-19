using UnityEngine;

public class VictoryDefeatPlayerDisplay : MonoBehaviour
{
    public AnimationClip victoryAnimation;
    public AnimationClip defeatAnimation;
    public bool isWinner;

    private Animation anim;

    void Start()
    {
        anim = gameObject.AddComponent<Animation>();
        anim.playAutomatically = true;

        if (isWinner && victoryAnimation != null)
        {
            anim.AddClip(victoryAnimation, "Victory");
            anim.Play("Victory");
        }
        else if (!isWinner && defeatAnimation != null)
        {
            anim.AddClip(defeatAnimation, "Defeat");
            anim.Play("Defeat");
        }
    }
}
