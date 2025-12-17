using System;
using System.Collections.Generic;
using System.Linq;
using substrate_shared.interfaces.Codex;

namespace substrate_shared.DTO.Codex
{
    public sealed class InMemoryCodexRepository : ICodexRepository
    {
        private readonly List<CodexEntry> _entries = [];

        public void Save(CodexEntry entry) => _entries.Add(entry);

        public CodexEntry GetById(string id) =>
            _entries.FirstOrDefault(e => e.Id == id) ?? throw new InvalidOperationException();

        public IEnumerable<CodexEntry> GetByChapter(int chapterIndex) =>
            _entries.Where(e => e.Data.ContainsKey("ChapterIndex") &&
                                (int)e.Data["ChapterIndex"] == chapterIndex);

        public IEnumerable<CodexEntry> GetAll() => _entries;
    }
}