﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Portal/PaginaMaestra.Master" AutoEventWireup="true" CodeBehind="Cesantias.aspx.cs" Inherits="PortalTrabajadores.Portal.Estadocesantias" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContainerTitulo" runat="server">
     <asp:UpdatePanel ID="UpdatePanel2" runat="server">
        <ContentTemplate>
            <asp:Label ID="lblTitulo" runat="server" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Container" runat="server">    
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <asp:GridView ID="GridView1" runat="server" DataSourceID="SqlDataSource1" GridLines="Vertical" AutoGenerateColumns="False" OnRowCommand="GridView1_RowCommand">
                <AlternatingRowStyle CssClass="ColorOscuro" />
                <Columns>
                    <asp:BoundField DataField="anio_cesantia" HeaderText="Año" >
                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                    </asp:BoundField>
                    <asp:BoundField DataField="mes_cesantia" HeaderText="Mes" Visible="false" >
                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                    </asp:BoundField>
                    <asp:TemplateField HeaderText="Descargar" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:ImageButton ID="Imgpdf" runat="server" ImageUrl="~/Img/pdf.gif" CommandArgument='<%#Eval("idCesantia") %>' CommandName="PDF" />
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:trabajadoresConnectionString %>"
                ProviderName="<%$ ConnectionStrings:trabajadoresConnectionString.ProviderName %>"></asp:SqlDataSource>                       
        </ContentTemplate>
         <Triggers>
            <asp:PostBackTrigger ControlID="GridView1"/>
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
