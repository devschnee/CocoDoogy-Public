using UnityEngine;

public class AnimalAnimationController : MonoBehaviour
{
    [SerializeField] AnimalType animalType;
    [SerializeField] Animator anim;

    private IAnimalAnimation animalAnim;

    private void Awake()
    {
        if (anim == null) anim = GetComponent<Animator>();
        switch (animalType)
        {
            case AnimalType.pig :
            animalAnim = new BaseInGameBoarAnimation();
            break;
            case AnimalType.turtle :
            animalAnim = new BaseInGameTurtleAnimation();
            break;
            case AnimalType.cow :
            animalAnim = new BaseInGameBuffaloAnimation();
            break;
        }

        animalAnim.Init(anim, this);
    }

    private void OnEnable()
    {
        animalAnim.OnEnabled();
    }

    private void Update()
    {
        animalAnim.Update();
    }

    private void OnDisable()
    {
        animalAnim.OnDisable();
    }

    public void HandleAnimationEvent(string animName)
    {
        animalAnim.HandleAnimEvent(animName);
    }

    public void HandleSoundEvent(string soundName)
    {
        animalAnim.HandleSoundEvent(soundName);
    }

}
