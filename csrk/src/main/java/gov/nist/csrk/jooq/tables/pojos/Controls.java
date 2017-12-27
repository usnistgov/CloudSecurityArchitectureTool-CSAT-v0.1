/*
 * This file is generated by jOOQ.
*/
package gov.nist.csrk.jooq.tables.pojos;


import java.io.Serializable;

import javax.annotation.Generated;


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
public class Controls implements Serializable {

    private static final long serialVersionUID = -695626899;

    private Integer id;
    private String  name;
    private Integer baselineid;
    private Integer familyid;
    private Integer priorityid;
    private String  description;
    private String  guidance;

    public Controls() {}

    public Controls(Controls value) {
        this.id = value.id;
        this.name = value.name;
        this.baselineid = value.baselineid;
        this.familyid = value.familyid;
        this.priorityid = value.priorityid;
        this.description = value.description;
        this.guidance = value.guidance;
    }

    public Controls(
        Integer id,
        String  name,
        Integer baselineid,
        Integer familyid,
        Integer priorityid,
        String  description,
        String  guidance
    ) {
        this.id = id;
        this.name = name;
        this.baselineid = baselineid;
        this.familyid = familyid;
        this.priorityid = priorityid;
        this.description = description;
        this.guidance = guidance;
    }

    public Integer getId() {
        return this.id;
    }

    public void setId(Integer id) {
        this.id = id;
    }

    public String getName() {
        return this.name;
    }

    public void setName(String name) {
        this.name = name;
    }

    public Integer getBaselineid() {
        return this.baselineid;
    }

    public void setBaselineid(Integer baselineid) {
        this.baselineid = baselineid;
    }

    public Integer getFamilyid() {
        return this.familyid;
    }

    public void setFamilyid(Integer familyid) {
        this.familyid = familyid;
    }

    public Integer getPriorityid() {
        return this.priorityid;
    }

    public void setPriorityid(Integer priorityid) {
        this.priorityid = priorityid;
    }

    public String getDescription() {
        return this.description;
    }

    public void setDescription(String description) {
        this.description = description;
    }

    public String getGuidance() {
        return this.guidance;
    }

    public void setGuidance(String guidance) {
        this.guidance = guidance;
    }

    @Override
    public String toString() {
        StringBuilder sb = new StringBuilder("Controls (");

        sb.append(id);
        sb.append(", ").append(name);
        sb.append(", ").append(baselineid);
        sb.append(", ").append(familyid);
        sb.append(", ").append(priorityid);
        sb.append(", ").append(description);
        sb.append(", ").append(guidance);

        sb.append(")");
        return sb.toString();
    }
}
