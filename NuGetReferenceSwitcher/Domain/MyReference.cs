using VSLangProj;

namespace RicoSuter.NuGetReferenceSwitcher.Domain
{
    public class MyReference
    {
        public MyReference(Reference reference)
        {
            Reference = reference;

            Name = reference.Name;
            Path = reference.Path;
        }

        public Reference Reference { get; private set; }

        public string Name { get; private set; }
        public string Path { get; set; }

        public string ProjectName
        {
            get { return Reference.SourceProject != null ? Reference.SourceProject.Name : null; }
        }
    }
}