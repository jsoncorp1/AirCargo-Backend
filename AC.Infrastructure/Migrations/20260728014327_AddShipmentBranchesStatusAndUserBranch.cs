using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AC.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddShipmentBranchesStatusAndUserBranch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "branch_office_id",
                table: "users",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "delivery_comment",
                table: "shipments",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "destination_branch_office_id",
                table: "shipments",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "observation",
                table: "shipments",
                type: "character varying(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "origin_branch_office_id",
                table: "shipments",
                type: "uuid",
                nullable: true);

            // Los envíos anteriores al modelo de estados quedan como InTransit;
            // un string vacío rompería la conversión a ShipmentStatus al leer.
            migrationBuilder.AddColumn<string>(
                name: "status",
                table: "shipments",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "InTransit");

            migrationBuilder.CreateIndex(
                name: "ix_users_branch_office_id",
                table: "users",
                column: "branch_office_id");

            migrationBuilder.CreateIndex(
                name: "ix_shipments_destination_branch_office_id",
                table: "shipments",
                column: "destination_branch_office_id");

            migrationBuilder.CreateIndex(
                name: "ix_shipments_origin_branch_office_id",
                table: "shipments",
                column: "origin_branch_office_id");

            migrationBuilder.AddForeignKey(
                name: "fk_shipments_branch_offices_destination_branch_office_id",
                table: "shipments",
                column: "destination_branch_office_id",
                principalTable: "branch_offices",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_shipments_branch_offices_origin_branch_office_id",
                table: "shipments",
                column: "origin_branch_office_id",
                principalTable: "branch_offices",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_users_branch_offices_branch_office_id",
                table: "users",
                column: "branch_office_id",
                principalTable: "branch_offices",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_shipments_branch_offices_destination_branch_office_id",
                table: "shipments");

            migrationBuilder.DropForeignKey(
                name: "fk_shipments_branch_offices_origin_branch_office_id",
                table: "shipments");

            migrationBuilder.DropForeignKey(
                name: "fk_users_branch_offices_branch_office_id",
                table: "users");

            migrationBuilder.DropIndex(
                name: "ix_users_branch_office_id",
                table: "users");

            migrationBuilder.DropIndex(
                name: "ix_shipments_destination_branch_office_id",
                table: "shipments");

            migrationBuilder.DropIndex(
                name: "ix_shipments_origin_branch_office_id",
                table: "shipments");

            migrationBuilder.DropColumn(
                name: "branch_office_id",
                table: "users");

            migrationBuilder.DropColumn(
                name: "delivery_comment",
                table: "shipments");

            migrationBuilder.DropColumn(
                name: "destination_branch_office_id",
                table: "shipments");

            migrationBuilder.DropColumn(
                name: "observation",
                table: "shipments");

            migrationBuilder.DropColumn(
                name: "origin_branch_office_id",
                table: "shipments");

            migrationBuilder.DropColumn(
                name: "status",
                table: "shipments");
        }
    }
}
