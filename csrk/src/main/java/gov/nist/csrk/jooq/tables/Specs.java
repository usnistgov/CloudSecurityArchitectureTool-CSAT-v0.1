/*
 * This file is generated by jOOQ.
*/
package gov.nist.csrk.jooq.tables;


import gov.nist.csrk.jooq.DefaultSchema;
import gov.nist.csrk.jooq.Keys;
import gov.nist.csrk.jooq.tables.records.SpecsRecord;

import java.util.Arrays;
import java.util.List;

import javax.annotation.Generated;

import org.jooq.Field;
import org.jooq.ForeignKey;
import org.jooq.Name;
import org.jooq.Schema;
import org.jooq.Table;
import org.jooq.TableField;
import org.jooq.UniqueKey;
import org.jooq.impl.DSL;
import org.jooq.impl.TableImpl;


/**
 * This class is generated by jOOQ.
 */
@Generated(
    value = {
        "http://www.jooq.org",
        "jOOQ version:3.10.2"
    },
    comments = "This class is generated by jOOQ"
)
@SuppressWarnings({ "all", "unchecked", "rawtypes" })
public class Specs extends TableImpl<SpecsRecord> {

    private static final long serialVersionUID = -1178306588;

    /**
     * The reference instance of <code>Specs</code>
     */
    public static final Specs SPECS = new Specs();

    /**
     * The class holding records for this type
     */
    @Override
    public Class<SpecsRecord> getRecordType() {
        return SpecsRecord.class;
    }

    /**
     * The column <code>Specs.Id</code>.
     */
    public final TableField<SpecsRecord, Integer> ID = createField("Id", org.jooq.impl.SQLDataType.INTEGER.nullable(false), this, "");

    /**
     * The column <code>Specs.ControlsId</code>.
     */
    public final TableField<SpecsRecord, Integer> CONTROLSID = createField("ControlsId", org.jooq.impl.SQLDataType.INTEGER.nullable(false), this, "");

    /**
     * The column <code>Specs.SecificationName</code>.
     */
    public final TableField<SpecsRecord, String> SECIFICATIONNAME = createField("SecificationName", org.jooq.impl.SQLDataType.CHAR(8000).nullable(false), this, "");

    /**
     * The column <code>Specs.Description</code>.
     */
    public final TableField<SpecsRecord, String> DESCRIPTION = createField("Description", org.jooq.impl.SQLDataType.CHAR(8000).nullable(false), this, "");

    /**
     * The column <code>Specs.Guidance</code>.
     */
    public final TableField<SpecsRecord, String> GUIDANCE = createField("Guidance", org.jooq.impl.SQLDataType.CHAR(8000).nullable(false), this, "");

    /**
     * Create a <code>Specs</code> table reference
     */
    public Specs() {
        this(DSL.name("Specs"), null);
    }

    /**
     * Create an aliased <code>Specs</code> table reference
     */
    public Specs(String alias) {
        this(DSL.name(alias), SPECS);
    }

    /**
     * Create an aliased <code>Specs</code> table reference
     */
    public Specs(Name alias) {
        this(alias, SPECS);
    }

    private Specs(Name alias, Table<SpecsRecord> aliased) {
        this(alias, aliased, null);
    }

    private Specs(Name alias, Table<SpecsRecord> aliased, Field<?>[] parameters) {
        super(alias, null, aliased, parameters, "");
    }

    /**
     * {@inheritDoc}
     */
    @Override
    public Schema getSchema() {
        return DefaultSchema.DEFAULT_SCHEMA;
    }

    /**
     * {@inheritDoc}
     */
    @Override
    public UniqueKey<SpecsRecord> getPrimaryKey() {
        return Keys.PK_SPECS;
    }

    /**
     * {@inheritDoc}
     */
    @Override
    public List<UniqueKey<SpecsRecord>> getKeys() {
        return Arrays.<UniqueKey<SpecsRecord>>asList(Keys.PK_SPECS);
    }

    /**
     * {@inheritDoc}
     */
    @Override
    public List<ForeignKey<SpecsRecord, ?>> getReferences() {
        return Arrays.<ForeignKey<SpecsRecord, ?>>asList(Keys.FK_SPECS_CONTROLS_1);
    }

    /**
     * {@inheritDoc}
     */
    @Override
    public Specs as(String alias) {
        return new Specs(DSL.name(alias), this);
    }

    /**
     * {@inheritDoc}
     */
    @Override
    public Specs as(Name alias) {
        return new Specs(alias, this);
    }

    /**
     * Rename this table
     */
    @Override
    public Specs rename(String name) {
        return new Specs(DSL.name(name), null);
    }

    /**
     * Rename this table
     */
    @Override
    public Specs rename(Name name) {
        return new Specs(name, null);
    }
}
