namespace MediaPlayer.ViewModels;

internal enum MessageTemplateType : byte { Information = 0, Warning, Error }

/// <summary>
/// 
/// </summary>
internal record class MessageTemplate(string Message, MessageTemplateType Type) { }