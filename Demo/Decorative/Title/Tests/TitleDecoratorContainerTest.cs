using UnityEngine;

namespace AstrotypeInspector.Demo
{
    public class TitleDecoratorContainerTest : MonoBehaviour
    {
        // [Header("Unity Header A")]
        // [Header("Unity Header B")]
        // [Header("Unity Header C")]
        // [Header("Unity Header D")]
        
        // [Title("Astrotype Title A")]
        // [Title("Astrotype Title B")]
        // [Title("Astrotype Title C")]
        // [Title("Astrotype Title D")]
        
        [Title("Astrotype Title A Astrotype Title A Astrotype Title A Astrotype Title A", Icon.GameObject)]
        [Header("Unity Header A")]
        [Header("Unity Header B")]
        [Header("Unity Header C")]
        [Title("Astrotype Title B")]
        [Title("Astrotype Title C", Bottom = true)]
        [Header("Unity Header D")]
        [Title("Astrotype Title D")]
        public float myString;
        
        
        private void Start()
        {
            
        }
    }
}
