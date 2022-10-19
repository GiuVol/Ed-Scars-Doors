using System.Collections;
using UnityEngine;

public class StatusComponent : MonoBehaviour
{
    private const float MinBlindnessLevelDecrementSpeed = .5f;
    private const float StandardBlindnessCooldownTime = 5;

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
    /// The greater this value is, the more it will take to increase the <c>CurrentBlindnessLevel</c>.
    /// This value should be clamped between 0 and 1.
    /// </summary>
    public float BlindnessResistence { get; private set; }

    public float BlindnessDuration { get; private set; }
    
    /// <summary>
    /// The greater this value is, the faster the <c>CurrentBlindnessLevel</c> will decrease.
    /// </summary>
    public float BlindnessLevelDecrementSpeed { get; private set; }

    [SerializeField]
    private bool _useCustomBlindnessCooldownTime;

    [SerializeField]
    private float _customBlindnessCooldownTime;

    private float BlindnessCooldownTime
    {
        get
        {
            return _useCustomBlindnessCooldownTime ? _customBlindnessCooldownTime : StandardBlindnessCooldownTime;
        }
    }

    /// <summary>
    /// This value represents whether the character is blinded or not.
    /// </summary>
    public bool IsBlinded { get; private set; }

    /// <summary>
    /// This value represents whether the character can be blinded or not.
    /// </summary>
    public bool CanBeBlinded { get; private set; }

    /// <summary>
    /// Type of delegate method that has to be called when the character is blinded.
    /// </summary>
    public delegate void OnBlinded();

    /// <summary>
    /// Delegate method that has to be called when the character is blinded.
    /// </summary>
    public OnBlinded OnBlindedDelegate { get; private set; }

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
    /// Stores the damage that will be inflicted on the corrosion.
    /// </summary>
    public float CorrosionDamage { get; private set; }

    /// <summary>
    /// Stores the time that passes between damages inflicted from corrosion.
    /// </summary>
    public float CorrosionDamageInterval { get; private set; }
    
    /// <summary>
    /// This value represents whether the character is corroded or not.
    /// </summary>
    public bool IsCorroded { get; private set; }

    /// <summary>
    /// Type of delegate method that has to be called when the character is corroded.
    /// </summary>
    public delegate void OnCorroded();

    /// <summary>
    /// Delegate method that has to be called when the character is corroded.
    /// </summary>
    public OnCorroded OnCorrodedDelegate { get; private set; }
    
    #endregion

    /// <summary>
    /// Stores whether the character can get a status.
    /// </summary>
    public bool IsImmune { get; set; }

    /// <summary>
    /// The coroutine that is handling the immunity.
    /// </summary>
    private Coroutine _temporaryImmunityCoroutine;
    
    private bool _initialized;

    /// <summary>
    /// This method is used to setup the <c>StatusComponent</c> with the desired values.
    /// </summary>
    /// <param name="maxBlindnesslevel">
    /// The desired value for <c>MaxBlindnesslevel</c>.
    /// </param>
    /// <param name="blindnessResistence">
    /// The desired value for <c>BlindnessResistence</c>
    /// pre: this value should be clamped between 0 and 1.
    /// </param>
    /// <param name="blindnessLevelDecrementSpeed">
    /// The desired value for <c>BlindnessLevelDecrementSpeed</c>.
    /// </param>
    /// <param name="maxCorrosionTime">
    /// The desired value for <c>MaxCorrosionTime</c>.
    /// </param>
    /// <param name="corrosionDamage">
    /// The corrosion damage that the character will recieve.
    /// </param>
    /// <param name="corrosionDamageInterval">
    /// How much time passes between damages inflicted from corrosion.
    /// </param>
    /// <param name="onBlindedDelegate">
    /// A method that will be called when the character is blinded.
    /// </param>
    /// <param name="onCorrodedDelegate">
    /// A method that will be called when the character is corroded.
    /// </param>
    public void Setup(float maxBlindnesslevel, float blindnessResistence, float blindnessDuration, 
                      float blindnessLevelDecrementSpeed, float maxCorrosionTime, 
                      float corrosionDamage, float corrosionDamageInterval, 
                      OnBlinded onBlindedDelegate = null, OnCorroded onCorrodedDelegate = null)
    {
        CurrentBlindnesslevel = 0;
        IsBlinded = false;
        CanBeBlinded = true;

        CorrosionTimeLeft = 0;
        IsCorroded = false;

        blindnessLevelDecrementSpeed = Mathf.Max(blindnessLevelDecrementSpeed, 1);
        
        MaxBlindnesslevel = maxBlindnesslevel;
        BlindnessResistence = blindnessResistence;
        BlindnessDuration = blindnessDuration;
        BlindnessLevelDecrementSpeed = blindnessLevelDecrementSpeed;
        OnBlindedDelegate = onBlindedDelegate;

        MaxCorrosionTime = maxCorrosionTime;
        CorrosionDamage = corrosionDamage;
        CorrosionDamageInterval = corrosionDamageInterval;
        OnCorrodedDelegate = onCorrodedDelegate;

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

        if (CorrosionTimeLeft > 0 && !IsCorroded)
        {
            IsCorroded = true;
            OnCorrodedDelegate?.Invoke();
        }

        if (CorrosionTimeLeft <= 0 && IsCorroded)
        {
            IsCorroded = false;
        }

        CurrentBlindnesslevel -= BlindnessLevelDecrementSpeed * Time.fixedDeltaTime;
        CurrentBlindnesslevel = Mathf.Clamp(CurrentBlindnesslevel, 0, MaxBlindnesslevel);

        CorrosionTimeLeft -= Time.fixedDeltaTime;
        CorrosionTimeLeft = Mathf.Clamp(CorrosionTimeLeft, 0, MaxCorrosionTime);
    }

    /// <summary>
    /// The IEnumerator used to inflict blindness when the <c>CurrentBlindnesslevel</c> reaches the <c>MaxBlindnesslevel</c>.
    /// This IEnumerator should be passed to the <c>StartCoroutine()</c> method, it automatically sets the blindness for the 
    /// correct time.
    /// </summary>
    private IEnumerator InflictBlindness()
    {
        if (!CanBeBlinded || IsImmune)
        {
            yield break;
        }

        IsBlinded = true;
        CanBeBlinded = false;

        OnBlindedDelegate?.Invoke();

        #region Deprecated

        /*

        float actualBlindnessLevelDecrementSpeed = Mathf.Max(BlindnessLevelDecrementSpeed, MinBlindnessLevelDecrementSpeed);
        float timeToWait = 
            Mathf.Min(BlindnessDuration, 
                      MaxBlindnesslevel / actualBlindnessLevelDecrementSpeed);

        yield return new WaitForSeconds(timeToWait);

        IsBlinded = false;

        */

        #endregion

        yield return new WaitForSeconds(BlindnessDuration);

        yield return new WaitUntil(() => CurrentBlindnesslevel == 0);

        IsBlinded = false;

        yield return new WaitForSeconds(BlindnessCooldownTime);

        CanBeBlinded = true;
    }

    /// <summary>
    /// This method is used to increase the <c>CurrentBlindnesslevel</c> of the character.
    /// </summary>
    /// <param name="increment">The desired increment</param>
    public void IncreaseBlindnessLevel(float increment)
    {
        if (!CanBeBlinded || IsImmune)
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
        if (IsImmune)
        {
            return;
        }

        CorrosionTimeLeft = Mathf.Min(MaxCorrosionTime, CorrosionTimeLeft + increment);
    }

    /// <summary>
    /// Method that can make the character immune to statuses for a while.
    /// </summary>
    /// <param name="time">The time that the character will be immune</param>
    public void SetImmunityTemporarily(float time)
    {
        if (_temporaryImmunityCoroutine == null)
        {
            _temporaryImmunityCoroutine = StartCoroutine(SetImmunityTemporarilyIEnumerator(time));
        }
    }

    /// <summary>
    /// IEnumerator that can make the character immune to statuses for a while.
    /// </summary>
    /// <param name="time">The time that the character will be immune</param>
    private IEnumerator SetImmunityTemporarilyIEnumerator(float time)
    {
        IsImmune = true;

        yield return new WaitForSeconds(time);

        IsImmune = false;

        _temporaryImmunityCoroutine = null;
    }
}
