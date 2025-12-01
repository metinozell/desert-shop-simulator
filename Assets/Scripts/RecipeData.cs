using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class IngredientRequirement
{
    public ItemData ingredient;
    public int quantity;
}

[System.Serializable]
public class BakeStep
{
    public string stepName;
    public float stepDuration;
    // public MiniGameType miniGame; // (İleride daha karmaşık hale getirilebilir)
}

[CreateAssetMenu(fileName = "NewRecipeData", menuName = "Dessert Shop/Recipe Data")]
public class RecipeData : ScriptableObject
{
    public string dessertName;
    [TextArea]
    public string description;
    public Sprite dessertImage;
    
    [Header("Gameplay")]
    public float baseReward;
    public int difficulty; 
    public float basePreparationTime; 

    [Header("Requirements")]
    public List<IngredientRequirement> requiredIngredients; 
    
    [Header("Baking Steps")]
    public List<BakeStep> bakeSteps; 
}