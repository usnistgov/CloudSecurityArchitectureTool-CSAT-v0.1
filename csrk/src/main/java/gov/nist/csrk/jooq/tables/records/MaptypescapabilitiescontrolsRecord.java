/*
 * This file is generated by jOOQ.
*/
package gov.nist.csrk.jooq.tables.records;


import gov.nist.csrk.jooq.tables.Maptypescapabilitiescontrols;

import javax.annotation.Generated;

import org.jooq.Field;
import org.jooq.Record1;
import org.jooq.Record6;
import org.jooq.Row6;
import org.jooq.impl.UpdatableRecordImpl;


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
public class MaptypescapabilitiescontrolsRecord extends UpdatableRecordImpl<MaptypescapabilitiescontrolsRecord> implements Record6<Integer, Integer, Integer, Integer, Integer, Boolean> {

    private static final long serialVersionUID = 776038779;

    /**
     * Setter for <code>MapTypesCapabilitiesControls.Id</code>.
     */
    public void setId(Integer value) {
        set(0, value);
    }

    /**
     * Getter for <code>MapTypesCapabilitiesControls.Id</code>.
     */
    public Integer getId() {
        return (Integer) get(0);
    }

    /**
     * Setter for <code>MapTypesCapabilitiesControls.CapabilitiesId</code>.
     */
    public void setCapabilitiesid(Integer value) {
        set(1, value);
    }

    /**
     * Getter for <code>MapTypesCapabilitiesControls.CapabilitiesId</code>.
     */
    public Integer getCapabilitiesid() {
        return (Integer) get(1);
    }

    /**
     * Setter for <code>MapTypesCapabilitiesControls.ControlsId</code>.
     */
    public void setControlsid(Integer value) {
        set(2, value);
    }

    /**
     * Getter for <code>MapTypesCapabilitiesControls.ControlsId</code>.
     */
    public Integer getControlsid() {
        return (Integer) get(2);
    }

    /**
     * Setter for <code>MapTypesCapabilitiesControls.MapTypesId</code>.
     */
    public void setMaptypesid(Integer value) {
        set(3, value);
    }

    /**
     * Getter for <code>MapTypesCapabilitiesControls.MapTypesId</code>.
     */
    public Integer getMaptypesid() {
        return (Integer) get(3);
    }

    /**
     * Setter for <code>MapTypesCapabilitiesControls.SpecsId</code>.
     */
    public void setSpecsid(Integer value) {
        set(4, value);
    }

    /**
     * Getter for <code>MapTypesCapabilitiesControls.SpecsId</code>.
     */
    public Integer getSpecsid() {
        return (Integer) get(4);
    }

    /**
     * Setter for <code>MapTypesCapabilitiesControls.IsControlMap</code>.
     */
    public void setIscontrolmap(Boolean value) {
        set(5, value);
    }

    /**
     * Getter for <code>MapTypesCapabilitiesControls.IsControlMap</code>.
     */
    public Boolean getIscontrolmap() {
        return (Boolean) get(5);
    }

    // -------------------------------------------------------------------------
    // Primary key information
    // -------------------------------------------------------------------------

    /**
     * {@inheritDoc}
     */
    @Override
    public Record1<Integer> key() {
        return (Record1) super.key();
    }

    // -------------------------------------------------------------------------
    // Record6 type implementation
    // -------------------------------------------------------------------------

    /**
     * {@inheritDoc}
     */
    @Override
    public Row6<Integer, Integer, Integer, Integer, Integer, Boolean> fieldsRow() {
        return (Row6) super.fieldsRow();
    }

    /**
     * {@inheritDoc}
     */
    @Override
    public Row6<Integer, Integer, Integer, Integer, Integer, Boolean> valuesRow() {
        return (Row6) super.valuesRow();
    }

    /**
     * {@inheritDoc}
     */
    @Override
    public Field<Integer> field1() {
        return Maptypescapabilitiescontrols.MAPTYPESCAPABILITIESCONTROLS.ID;
    }

    /**
     * {@inheritDoc}
     */
    @Override
    public Field<Integer> field2() {
        return Maptypescapabilitiescontrols.MAPTYPESCAPABILITIESCONTROLS.CAPABILITIESID;
    }

    /**
     * {@inheritDoc}
     */
    @Override
    public Field<Integer> field3() {
        return Maptypescapabilitiescontrols.MAPTYPESCAPABILITIESCONTROLS.CONTROLSID;
    }

    /**
     * {@inheritDoc}
     */
    @Override
    public Field<Integer> field4() {
        return Maptypescapabilitiescontrols.MAPTYPESCAPABILITIESCONTROLS.MAPTYPESID;
    }

    /**
     * {@inheritDoc}
     */
    @Override
    public Field<Integer> field5() {
        return Maptypescapabilitiescontrols.MAPTYPESCAPABILITIESCONTROLS.SPECSID;
    }

    /**
     * {@inheritDoc}
     */
    @Override
    public Field<Boolean> field6() {
        return Maptypescapabilitiescontrols.MAPTYPESCAPABILITIESCONTROLS.ISCONTROLMAP;
    }

    /**
     * {@inheritDoc}
     */
    @Override
    public Integer component1() {
        return getId();
    }

    /**
     * {@inheritDoc}
     */
    @Override
    public Integer component2() {
        return getCapabilitiesid();
    }

    /**
     * {@inheritDoc}
     */
    @Override
    public Integer component3() {
        return getControlsid();
    }

    /**
     * {@inheritDoc}
     */
    @Override
    public Integer component4() {
        return getMaptypesid();
    }

    /**
     * {@inheritDoc}
     */
    @Override
    public Integer component5() {
        return getSpecsid();
    }

    /**
     * {@inheritDoc}
     */
    @Override
    public Boolean component6() {
        return getIscontrolmap();
    }

    /**
     * {@inheritDoc}
     */
    @Override
    public Integer value1() {
        return getId();
    }

    /**
     * {@inheritDoc}
     */
    @Override
    public Integer value2() {
        return getCapabilitiesid();
    }

    /**
     * {@inheritDoc}
     */
    @Override
    public Integer value3() {
        return getControlsid();
    }

    /**
     * {@inheritDoc}
     */
    @Override
    public Integer value4() {
        return getMaptypesid();
    }

    /**
     * {@inheritDoc}
     */
    @Override
    public Integer value5() {
        return getSpecsid();
    }

    /**
     * {@inheritDoc}
     */
    @Override
    public Boolean value6() {
        return getIscontrolmap();
    }

    /**
     * {@inheritDoc}
     */
    @Override
    public MaptypescapabilitiescontrolsRecord value1(Integer value) {
        setId(value);
        return this;
    }

    /**
     * {@inheritDoc}
     */
    @Override
    public MaptypescapabilitiescontrolsRecord value2(Integer value) {
        setCapabilitiesid(value);
        return this;
    }

    /**
     * {@inheritDoc}
     */
    @Override
    public MaptypescapabilitiescontrolsRecord value3(Integer value) {
        setControlsid(value);
        return this;
    }

    /**
     * {@inheritDoc}
     */
    @Override
    public MaptypescapabilitiescontrolsRecord value4(Integer value) {
        setMaptypesid(value);
        return this;
    }

    /**
     * {@inheritDoc}
     */
    @Override
    public MaptypescapabilitiescontrolsRecord value5(Integer value) {
        setSpecsid(value);
        return this;
    }

    /**
     * {@inheritDoc}
     */
    @Override
    public MaptypescapabilitiescontrolsRecord value6(Boolean value) {
        setIscontrolmap(value);
        return this;
    }

    /**
     * {@inheritDoc}
     */
    @Override
    public MaptypescapabilitiescontrolsRecord values(Integer value1, Integer value2, Integer value3, Integer value4, Integer value5, Boolean value6) {
        value1(value1);
        value2(value2);
        value3(value3);
        value4(value4);
        value5(value5);
        value6(value6);
        return this;
    }

    // -------------------------------------------------------------------------
    // Constructors
    // -------------------------------------------------------------------------

    /**
     * Create a detached MaptypescapabilitiescontrolsRecord
     */
    public MaptypescapabilitiescontrolsRecord() {
        super(Maptypescapabilitiescontrols.MAPTYPESCAPABILITIESCONTROLS);
    }

    /**
     * Create a detached, initialised MaptypescapabilitiescontrolsRecord
     */
    public MaptypescapabilitiescontrolsRecord(Integer id, Integer capabilitiesid, Integer controlsid, Integer maptypesid, Integer specsid, Boolean iscontrolmap) {
        super(Maptypescapabilitiescontrols.MAPTYPESCAPABILITIESCONTROLS);

        set(0, id);
        set(1, capabilitiesid);
        set(2, controlsid);
        set(3, maptypesid);
        set(4, specsid);
        set(5, iscontrolmap);
    }
}
