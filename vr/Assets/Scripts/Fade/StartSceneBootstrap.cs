using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Fade
{
    public class StartSceneBootstrap : MonoBehaviour
    {
        void Start()
        {
            SceneSequenceManager.Instance.PreloadNextScene();
        }
    }
}
