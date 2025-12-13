using System.Collections.Generic;
using substrate_shared.DTO.Codex;

namespace substrate_shared.interfaces.Codex
{
    /// <summary>
    /// Defines persistence operations for CodexEntries.
    /// </summary>
    public interface ICodexRepository
    {
        void Save(CodexEntry entry);
        CodexEntry GetById(string id);
        IEnumerable<CodexEntry> GetByChapter(int chapterIndex);
        IEnumerable<CodexEntry> GetAll();
    }
}