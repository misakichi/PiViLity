using PiViLityCore.Shell;

namespace PiViLityCore.Shell
{
    public class DirTree
    {
        public DirTreeNode RootNode;

        public DirTree()
        {
            RootNode = new(this);
            RootNode.SetType(DirTreeNodeType.ThisPC, null);
        }



    }
}
