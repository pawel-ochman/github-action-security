using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using GithubActionsDotnet.Common.Service;
using GithubActionsDotnet.Security.Actions;
using GithubActionsDotnet.Security.Models;
using GithubActionsDotnet.Security.Services;
using Moq;
using Shouldly;
using Xunit;

namespace GithubActionsDotnet.Security.Tests
{
    public class Class1
    {
        private readonly GenerateCodeScanningPdfConfiguration _config;
        private readonly Mock<IGithubClient> _githubClientMock;
        private readonly Mock<IPdfGeneration> _pdfGenerationMock;
        private readonly GenerateCodeScanningPdf _subject;

        public Class1()
        {
            _config = new GenerateCodeScanningPdfConfiguration
            {
                TemplateUrl = "C:\\Projects\\github-action-security\\src\\GithubActionsDotnet\\Templates\\default.html",
                RepositoryUrl = "test repository url"

            };
            _githubClientMock = new Mock<IGithubClient>();
            _pdfGenerationMock = new Mock<IPdfGeneration>();

            _subject = new GenerateCodeScanningPdf(
                _config,
                _githubClientMock.Object,
                _pdfGenerationMock.Object
            );
        }

        [Fact]
        public void test()
        {   
        }
    }

    public static class T
    {
        public static Stream ToStream(this string text)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(text);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}