// SnapshotManifest consumes these value holders and the text-sink interface only.
// This does not replace or exercise SnapshotPartCache.Compose or world serialization.
namespace Remains.Persistence
{
    public static class SnapshotPartCache
    {
        public interface ITextSink {void Append(string text,int start,int count);void Append(char value);}
        public sealed class PartSet
        {
            public readonly string Name;
            public readonly string[] Keys,Texts;
            public PartSet(string name,string[] keys,string[] texts) {Name=name;Keys=keys;Texts=texts;}
        }
    }
}
