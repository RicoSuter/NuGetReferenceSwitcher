using VSLangProj;

namespace NuGetReferenceSwitcher.Presentation.Domain
{
    public class ReferenceModel
    {
        private readonly Reference _reference;

        public ReferenceModel(Reference reference)
        {
            _reference = reference;

            Name = reference.Name;
            Path = reference.Path;
        }

        public string Name { get; private set; }

        public string Path { get; set; }

        public string ProjectName
        {
            get
            {
                return _reference.SourceProject != null ? _reference.SourceProject.Name : null;
            }
        }

        public void Remove()
        {
            _reference.Remove();
        }
    }
}