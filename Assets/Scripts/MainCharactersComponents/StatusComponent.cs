using System.Collections;
using UnityEngine;

public class StatusComponent : MonoBehaviour
{
    private const float MinBlindnessLevelDecrementSpeed = .5f;

    #region Blindness

    /// <summary>
    /// The current blindness level of the character with this <c>StatusComponent</c> attached.
    /// </summary>
    public float CurrentBlindnesslevel { get; private set; }

    /// <summary>
    /// The max blindness level of the character with this <c>StatusComponent</c> attached. 
    /// When the <c>CurrentBlindnessLevel</c> reaches this value, the character will be blinded.
    /// </summary>
    public float MaxBlindnesslevel { get; private set; }

    /// <summary>
    /// This value determines how long the character remains blinded.
    /// </summary>
    public float BlindnessDuration { get; private set; }

    /// <summary>
    /// This value determines how long it takes to increase the <c>CurrentBlindnesslevel</c> again 
    /// after the <c>CurrentBlindnesslevel</c> reaches the 0.
    /// </summary>
    public float BlindnessCooldownTime { get; private set; }

    /// <summary>
    /// The greater this value is, the more it will take to increase the <c>CurrentBlindnessLevel</c>.
    /// This value should be clamped between 0 and 1.
    /// </summary>
    public float BlindnessResistence { get; private set; }

    /// <summary>
    /// The greater this value is, the faster the <c>CurrentBlindnessLevel</c> will decrease.
    /// </summary>
    public float BlindnessLevelDecrementSpeed { get; private set; }

    /// <summary>
    /// This value represents whether the character is blinded or not.
    /// </summary>
    public bool IsBlinded { get; private set; }

    /// <summary>
    /// This value represents whether the character can be blinded or not.
    /// </summary>
    public bool CanBeBlinded { get; private set; }

    #endregion

    #region Corrosion

    /// <summary>
    /// The corrosion time left.
    /// </summary>
    public float CorrosionTimeLeft { get; private set; }

    /// <summary>
    /// The max corrosion time that the character can reach.
    /// </summary>
    public float MaxCorrosionTime { get; private set; }

    /// <summary>
    /// The greater this value is, the less the character will be damaged from corrosion.
    /// This value should be clamped between 0 and 1.
    /// </summary>
    public float CorrosionResistence { get; private set; }

    /// <summary>
    /// This value represents whether the character is corroded or not.
    /// </summary>
    public bool IsCorroded { get; private set; }

    #endregion

    private bool _initialized;

    /// <summary>
    /// This method is used to setup the <c>StatusComponent</c> with the desired values.
    /// </summary>
    /// <param name="maxBlindnesslevel">
    /// The desired value for <c>MaxBlindnesslevel</c>
    /// </param>
    /// <param name="blindnessDuration">
    /// The desired value for <c>BlindnessDuration</c>
    /// </param>
    /// <param name="blindnessCooldownTime">
    /// The desired value for <c>BlindnessCooldownTime</c>
    /// </param>
    /// <param name="blindnessResistence">
    /// The desired value for <c>BlindnessResistence</c>
    /// pre: this value should be clamped between 0 and 1.
    /// </param>
    /// <param name="blindnessLevelDecrementSpeed">
    /// The desired value for <c>BlindnessLevelDecrementSpeed</c>
    /// </param>
    /// <param name="maxCorrosionTime">
    /// The desired value for <c>MaxCorrosionTime</c>
    /// </param>
    /// <param name="corrosionResistence">
    /// The desired value for <c>CorrosionResistence</c>
    /// pre: this value should be clamped between 0 and 1.
    /// </param>
    public void Setup(float maxBlindnesslevel, float blindnessDuration, 
                      float blindnessCooldownTime, float blindnessResistence, 
                      float blindnessLevelDecrementSpeed, float maxCorrosionTime, 
                      float corrosionResistence)
    {
        CurrentBlindnesslevel = 0;
        IsBlinded = false;
        CanBeBlinded = true;

        CorrosionTimeLeft = 0;
        IsCorroded = false;

        blindnessLevelDecrementSpeed = Mathf.Max(blindnessLevelDecrementSpeed, 1);
        
        MaxBlindnesslevel = maxBlindnesslevel;
        BlindnessDuration = Mathf.Min(blindnessDuration, maxBlindnesslevel / blindnessLevelDecrementSpeed);
        BlindnessCooldownTime = blindnessCooldownTime;
        BlindnessResistence = blindnessResistence;
        BlindnessLevelDecrementSpeed = blindnessLevelDecrementSpeed;

        MaxCorrosionTime = maxCorrosionTime;
        CorrosionResistence = corrosionResistence;

        _initialized = true;
    }

    void FixedUpdate()
    {
        if (!_initialized)
        {
            return;
        }

        if (CurrentBlindnesslevel >= MaxBlindnesslevel && !IsBlinded)
        {
            StartCoroutine(InflictBlindness());
        }

        CurrentBlindnesslevel -= BlindnessLevelDecrementSpeed * Time.fixedDeltaTime;
        CurrentBlindnesslevel = Mathf.Clamp(CurrentBlindnesslevel, 0, MaxBlindnesslevel);

        CorrosionTimeLeft -= Time.fixedDeltaTime;
        CorrosionTimeLeft = Mathf.Clamp(CorrosionTimeLeft, 0, MaxCorrosionTime);

        IsCorroded = CorrosionTimeLeft > 0;
    }

    /// <summary>
    /// The IEnumerator used to inflict blindness when the <c>CurrentBlindnesslevel</c> reaches the <c>MaxBlindnesslevel</c>.
    /// This IEnumerator should be passed to the <c>StartCoroutine()</c> method, it automatically sets the blindness for the 
    /// correct time.
    /// </summary>
    private IEnumerator InflictBlindness()
    {
        IsBlinded = true;
        CanBeBlinded = false;

        float actualBlindnessLevelDecrementSpeed = Mathf.Max(BlindnessLevelDecrementSpeed, MinBlindnessLevelDecrementSpeed);
        float timeToWait = 
            Mathf.Min(BlindnessDuration, 
                      MaxBlindnesslevel / actualBlindnessLevelDecrementSpeed);

        yield return new WaitForSeconds(timeToWait);

        IsBlinded = false;

        yield return new WaitUntil(() => CurrentBlindnesslevel == 0);

        yield return new WaitForSeconds(BlindnessCooldownTime);

        CanBeBlinded = true;
    }

    /// <summary>
    /// This method is used to increase the <c>CurrentBlindnesslevel</c> of the character.
    /// </summary>
    /// <param name="increment">The desired increment</param>
    public void IncreaseBlindnessLevel(float increment)
    {
        if (CanBeBlinded == false)
        {
            return;
        }

        increment = Mathf.Max(increment, 0);
        float actualBlindnessResistence = Mathf.Clamp01(BlindnessResistence);

        increment = (1 - actualBlindnessResistence) * increment;

        CurrentBlindnesslevel = Mathf.Min(MaxBlindnesslevel, CurrentBlindnesslevel + increment);
    }

    /// <summary>
    /// This method is used to increase the <c>CorrosionTimeLeft</c> of the character.
    /// </summary>
    /// <param name="increment">The desired increment</param>
    public void IncreaseCorrosionTime(float increment)
    {
        if (CorrosionTimeLeft > 0)
        {
            return;
        }

        CorrosionTimeLeft = Mathf.Min(MaxCorrosionTime, CorrosionTimeLeft + increment);
    }
}
