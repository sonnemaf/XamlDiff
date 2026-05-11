using XamlDiff.Library.Models;

namespace XamlDiff.Test;

[TestClass]
public partial class DiffTests {

    [TestMethod]
    public void TestTextAttributeChange() {
        // Arrange
        var diff = new Diff();

        // Act
        diff.Generate(""""
            <Page xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
                <Grid>
                    <TextBlock x:Name="TextBlock1" Text="Hello" />
                </Grid>
            </Page>
            """",
            """"
            <Page xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
                <Grid>
                    <TextBlock x:Name="TextBlock1" Text="World" />
                </Grid>
            </Page>
            """"
            , false, false, false, false, false);

        // Assert
        var expected = """
            <VisualState.Setters>
                <Setter Target="TextBlock1.(TextBlock.Text)" Value="World" />
            </VisualState.Setters>

            """;

        Assert.AreEqual(expected, diff.Output);
        Assert.IsEmpty(diff.Errors);
    }

    [TestMethod]
    public void TestPropertyAttributeToElementSyntax() {
        // Arrange
        var diff = new Diff();

        // Act
        diff.Generate(""""
            <Page xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
                <Grid>
                    <TextBlock x:Name="TextBlock1" Text="Hello" />
                </Grid>
            </Page>
            """",
            """"
            <Page xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
                <Grid>
                    <TextBlock x:Name="TextBlock1">
                        <TextBlock.Text>World</TextBlock.Text>
                    </TextBlock>
                </Grid>
            </Page>
            """"
            , false, false, false, false, false);

        // Assert
        var expected = """
            <VisualState.Setters>
                <Setter Target="TextBlock1.(TextBlock.Text)" Value="World" />
            </VisualState.Setters>

            """;

        Assert.AreEqual(expected, diff.Output);
        Assert.IsEmpty(diff.Errors);
    }

    [TestMethod]
    public void TestPropertyElementSyntaxToAttribute() {
        // Arrange
        var diff = new Diff();

        // Act
        diff.Generate(""""
            <Page xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
                <Grid>
                    <TextBlock x:Name="TextBlock1">
                        <TextBlock.Text>Hello</TextBlock.Text>
                    </TextBlock>
                </Grid>
            </Page>
            """",
            """"
            
            <Page xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
                <Grid>
                    <TextBlock x:Name="TextBlock1" Text="World" />
                </Grid>
            </Page>
            """"
            , false, false, false, false, false);

        // Assert
        var expected = """
            <VisualState.Setters>
                <Setter Target="TextBlock1.(TextBlock.Text)" Value="World" />
            </VisualState.Setters>

            """;

        Assert.AreEqual(expected, diff.Output);
        Assert.IsEmpty(diff.Errors);
    }


    [TestMethod]
    public void TestTextAttributeEmpty() {
        // Arrange
        var diff = new Diff();

        // Act
        diff.Generate(""""
            <Page xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
                <Grid>
                    <TextBlock x:Name="TextBlock1" Text="Hello" />
                </Grid>
            </Page>
            """",
            """"
            <Page xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
                <Grid>
                    <TextBlock x:Name="TextBlock1" />
                </Grid>
            </Page>
            """"
            , false, false, false, false, false);

        // Assert
        var expected = """
            <VisualState.Setters>
                <Setter Target="TextBlock1.(TextBlock.Text)" Value="{x:Null}" />
            </VisualState.Setters>

            """;

        Assert.AreEqual(expected, diff.Output);
        Assert.IsEmpty(diff.Errors);
    }

    [TestMethod]
    public void TestTextElementEmptyDest() {
        // Arrange
        var diff = new Diff();

        // Act
        diff.Generate(""""
            <Page xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
                <Grid>
                    <TextBlock x:Name="TextBlock1">
                        <TextBlock.Text>Hello</TextBlock.Text>
                    </TextBlock>
                </Grid>
            </Page>
            """",
            """"
            <Page xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
                <Grid>
                    <TextBlock x:Name="TextBlock1" />
                </Grid>
            </Page>
            """"
            , false, false, false, false, false);

        // Assert
        var expected = """
            <VisualState.Setters>
                <Setter Target="TextBlock1.(TextBlock.Text)" Value="{x:Null}" />
            </VisualState.Setters>

            """;

        Assert.AreEqual(expected, diff.Output);
        Assert.IsEmpty(diff.Errors);
    }


    [TestMethod]
    public void TestTextElementEmptySource() {
        // Arrange
        var diff = new Diff();

        // Act
        diff.Generate(""""
            <Page xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
                <Grid>
                    <TextBlock x:Name="TextBlock1" />
                </Grid>
            </Page>
            """",
            """"
            <Page xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
                <Grid>
                    <TextBlock x:Name="TextBlock1">
                        <TextBlock.Text>Hello</TextBlock.Text>
                    </TextBlock>
                </Grid>
            </Page>
            """"
            , false, false, false, false, false);

        // Assert
        var expected = """
            <VisualState.Setters>
                <Setter Target="TextBlock1.(TextBlock.Text)" Value="Hello" />
            </VisualState.Setters>

            """;

        Assert.AreEqual(expected, diff.Output);
        Assert.IsEmpty(diff.Errors);
    }

    [TestMethod]
    public void TestAttachedPropertyAttribute() {
        // Arrange
        var diff = new Diff();

        // Act
        diff.Generate(""""
            <Page xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
                <Grid>
                    <TextBlock x:Name="TextBlock1" Grid.Row="1" />
                </Grid>
            </Page>
            """",
            """"
            <Page xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
                <Grid>
                    <TextBlock x:Name="TextBlock1" Grid.Row="2" />
                </Grid>
            </Page>
            """"
            , false, false, false, false, false);

        // Assert
        var expected = """
            <VisualState.Setters>
                <Setter Target="TextBlock1.(Grid.Row)" Value="2" />
            </VisualState.Setters>

            """;

        Assert.AreEqual(expected, diff.Output);
        Assert.IsEmpty(diff.Errors);
    }

    [TestMethod]
    public void TestAttachedPropertyAttributeMissingSource() {
        // Arrange
        var diff = new Diff();

        // Act
        diff.Generate(""""
            <Page xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
                <Grid>
                    <TextBlock x:Name="TextBlock1" />
                </Grid>
            </Page>
            """",
            """"
            <Page xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
                <Grid>
                    <TextBlock x:Name="TextBlock1" Grid.Row="1" />
                </Grid>
            </Page>
            """"
            , false, false, false, false, false);

        // Assert
        var expected = """
            <VisualState.Setters>
                <Setter Target="TextBlock1.(Grid.Row)" Value="1" />
            </VisualState.Setters>

            """;

        Assert.AreEqual(expected, diff.Output);
        Assert.IsEmpty(diff.Errors);
    }

    [TestMethod]
    public void TestAttachedPropertyAttributeMissingDest() {
        // Arrange
        var diff = new Diff();

        // Act
        diff.Generate(""""
            <Page xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
                <Grid>
                    <TextBlock x:Name="TextBlock1" Grid.Row="1" />
                </Grid>
            </Page>
            """",
            """"
            <Page xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
                <Grid>
                    <TextBlock x:Name="TextBlock1" />
                </Grid>
            </Page>
            """"
            , false, false, false, false, false);

        // Assert
        var expected = """
            <VisualState.Setters>
                <Setter Target="TextBlock1.(Grid.Row)" Value="0" />
            </VisualState.Setters>

            """;

        Assert.AreEqual(expected, diff.Output);
        Assert.IsEmpty(diff.Errors);
    }

    [TestMethod]
    public void TestAttachedPropertyAttribute2() {
        // Arrange
        var diff = new Diff();

        // Act
        diff.Generate(""""
            <Page xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
                <Grid>
                    <TextBlock x:Name="TextBlock1" Grid.Row="1" />
                </Grid>
            </Page>
            """",
            """"
            <Page xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
                <Grid>
                    <TextBlock x:Name="TextBlock1">
                        <Grid.Row>2</Grid.Row>
                    </TextBlock>
                </Grid>
            </Page>
            """"
            , false, false, false, false, false);

        // Assert
        var expected = """
            <VisualState.Setters>
                <Setter Target="TextBlock1.(Grid.Row)" Value="2" />
            </VisualState.Setters>

            """;

        Assert.AreEqual(expected, diff.Output);
        Assert.IsEmpty(diff.Errors);
    }

    [TestMethod]
    public void TestAttachedPropertyAttribute2b() {
        // Arrange
        var diff = new Diff();

        // Act
        diff.Generate(""""
            <Page xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
                <Grid>
                    <TextBlock x:Name="TextBlock1" />
                </Grid>
            </Page>
            """",
            """"
            <Page xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
                <Grid>
                    <TextBlock x:Name="TextBlock1">
                        <Grid.Row>2</Grid.Row>
                    </TextBlock>
                </Grid>
            </Page>
            """"
            , false, false, false, false, false);

        // Assert
        var expected = """
            <VisualState.Setters>
                <Setter Target="TextBlock1.(Grid.Row)" Value="2" />
            </VisualState.Setters>

            """;

        Assert.AreEqual(expected, diff.Output);
        Assert.IsEmpty(diff.Errors);
    }

    [TestMethod]
    public void TestAttachedPropertyAttribute3() {
        // Arrange
        var diff = new Diff();

        // Act
        diff.Generate(""""
            <Page xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
                <Grid>
                     <TextBlock x:Name="TextBlock1">
                        <Grid.Row>1</Grid.Row>
                    </TextBlock>
                </Grid>
            </Page>
            """",
            """"
            <Page xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
                <Grid>
                    <TextBlock x:Name="TextBlock1" Grid.Row="2" />
                </Grid>
            </Page>
            """"
            , false, false, false, false, false);

        // Assert
        var expected = """
            <VisualState.Setters>
                <Setter Target="TextBlock1.(Grid.Row)" Value="2" />
            </VisualState.Setters>

            """;

        Assert.AreEqual(expected, diff.Output);
        Assert.IsEmpty(diff.Errors);
    }

    // Use the UITestMethod attribute for tests that need to run on the UI thread.
    //[UITestMethod]
    //public void TestPropertyElementSyntaxDiff()
    //{
    //    var grid = new Grid();
    //    Assert.AreEqual(0, grid.MinWidth);
    //}
}
