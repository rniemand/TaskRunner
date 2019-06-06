using NUnit.Framework;
using TaskRunner.Core.Extensions;

namespace TaskRunner.Tests.Extensions.StringExtensionsTests
{
  [TestFixture]
  public class StripTrailingCharacterTests
  {
    [Test]
    public void StripTrailingCharacter_GivenNull_ShouldReturnEmptyString()
    {
      Assert.AreEqual(string.Empty, StringExtensions.StripTrailingCharacter(null));
    }

    [Test]
    public void StripTrailingCharacter_GivenEmptyString_ShouldReturnEmptyString()
    {
      Assert.AreEqual(string.Empty, "".StripTrailingCharacter());
    }

    [Test]
    public void StripTrailingCharacter_GivenNoTrailingSlash_ShouldReturnOriginalString()
    {
      Assert.AreEqual("hello", "hello".StripTrailingCharacter());
    }

    [Test]
    public void StripTrailingCharacter_GivenTrailingSlash_ShouldStripTrailingSlash()
    {
      Assert.AreEqual("hello", "hello\\".StripTrailingCharacter());
    }

    [Test]
    public void StripTrailingCharacter_GivenCustomCharacter_ShouldStripCharacter()
    {
      Assert.AreEqual("hi", "hi@".StripTrailingCharacter("@"));
    }

    [Test]
    public void StripTrailingCharacter_GivenLongCharacter_ShouldStripCharacter()
    {
      Assert.AreEqual("hi", "hi@@@".StripTrailingCharacter("@@@"));
    }

    [Test]
    public void StripTrailingCharacter_GivenCharacterNotFound_ShouldReturnOriginalString()
    {
      Assert.AreEqual("hi!!!", "hi!!!".StripTrailingCharacter("@@@"));
    }
  }
}
