using System;

namespace EF_code_first.Properties.Exceptions;

public class NotFoundException(string message) : Exception(message);
