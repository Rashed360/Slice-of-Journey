using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class RecipeSO : ScriptableObject {

    public List<KitchenObjectSO> kitchenObjectSOList;
    public string recipeName;
    public int recipePrice;
    public int recipeTime;
    public bool requiresBaking;

}