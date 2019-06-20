using System;

namespace Serilog.Enrichers.Private.Ecs.Models
{
    /// <summary>
    /// A file is defined as a set of information that has been created on, or has existed on a filesystem.
    /// File objects can be associated with host events, network events, and/or file events
    /// (e.g., those produced by File Integrity Monitoring [FIM] products or services).
    /// File fields provide details about the affected file associated with the event or metric.
    /// </summary>
    public class FileModel
    {
        /// <summary>
        /// Last time file metadata changed.
        /// type: date
        /// </summary>
        public DateTime? CTime { get; set; }

        /// <summary>
        /// Device that is the source of the file.
        /// type: keyword
        /// </summary>
        public string Device { get; set; }

        /// <summary>
        /// File extension.
        /// This should allow easy filtering by file extensions.
        /// type: keyword
        /// example: png
        /// </summary>
        public string Extension { get; set; }

        /// <summary>
        /// Primary group ID (GID) of the file.
        /// type: keyword
        /// </summary>
        public string GId { get; set; }

        /// <summary>
        /// Primary group name of the file.
        /// type: keyword
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// Inode representing the file in the filesystem.
        /// type: keyword
        /// </summary>
        public string Inode { get; set; }

        /// <summary>
        /// Mode of the file in octal representation.
        /// type: keyword
        /// example: 416
        /// </summary>
        public string Mode { get; set; }

        /// <summary>
        /// Last time file content was modified.
        /// type: date
        /// </summary>
        public DateTime? MTime { get; set; }

        /// <summary>
        /// File owner’s username.
        /// type: keyword
        /// </summary>
        public string Owner { get; set; }

        /// <summary>
        /// Path to the file.
        /// type: keyword
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// File size in bytes (field is only added when type is file).
        /// type: long
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        /// Target path for symlinks.
        /// type: keyword
        /// </summary>
        public string TargetPath { get; set; }

        /// <summary>
        /// File type (file, dir, or symlink).
        /// type: keyword
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// The user ID (UID) or security identifier (SID) of the file owner.
        /// type: keyword
        /// </summary>
        public string Uid { get; set; }
    }
}