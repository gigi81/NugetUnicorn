using System.Collections.Generic;

namespace NugetUnicorn.Business.SourcesParser.ProjectParser.Sax.Parser
{
    public class ContentHolder
    {
        public ContentHolder Parent { get; }

        private readonly List<SaxEvent> _content;

        public ContentHolder() : this(null)
        {
        }

        public ContentHolder(ContentHolder parent)
        {
            _content = new List<SaxEvent>();
            Parent = parent;
        }

        public void Append(SaxEvent contentItem)
        {
            _content.Add(contentItem);
        }

        public IReadOnlyCollection<SaxEvent> GetContent()
        {
            return _content.AsReadOnly();
        }
    }
}