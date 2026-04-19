using UnityEngine;

namespace AstrotypeInspector.Demo
{
    public class TitleDecorationTest_SeparateAttributes : MonoBehaviour
    {
        public enum Sex { Male, Female, Other }
        
        [Title("Pilot Registration")]
        [Subtitle("Register for ATLAS Racing Championship 2200 Season")]
        [Separator]
        
        [AddSpace]
        [Subtitle("Full name")]
        [SerializeField] private string firstName;
        [SerializeField] private string middleName;
        [SerializeField] private string lastName;
        
        [Space]
        [Subtitle("Bio details")]
        [SerializeField] private int age;
        [SerializeField] private Sex sex;
    }
}
